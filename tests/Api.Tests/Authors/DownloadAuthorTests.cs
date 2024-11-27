using System.Net;
using FastEndpoints;
using FastEndpoints.Testing;
using FluentAssertions;
using TobyMeehan.Com.Domain.Downloads;
using TobyMeehan.Com.Features.Authors;
using TobyMeehan.Com.Features.Downloads;
using CreateDownload = TobyMeehan.Com.Features.Downloads.Post;
using GetAuthors = TobyMeehan.Com.Features.Authors.GetByDownload;
using GetAuthor = TobyMeehan.Com.Features.Authors.GetById;
using AddAuthor = TobyMeehan.Com.Features.Authors.Post;
using KickAuthor = TobyMeehan.Com.Features.Authors.Delete;

namespace Api.Tests.Authors;

public class DownloadAuthorTests : TestBase<ApiApp>
{
    private readonly ApiApp _app;

    public DownloadAuthorTests(ApiApp app)
    {
        _app = app;
    }

    [Fact, Priority(1)]
    public async Task CreateDownload_ShouldAddOwnerAuthor()
    {
        var (response1, download) =
            await _app.UserA.POSTAsync<CreateDownload.Endpoint, CreateDownload.Request, DownloadResponse>(new()
            {
                Title = Fake.Commerce.ProductName(),
                Summary = Fake.Commerce.ProductDescription(),
                Description = Fake.Lorem.Paragraphs(),
                Visibility = Fake.PickRandom<Visibility>()
            });

        response1.IsSuccessStatusCode.Should().BeTrue();
        
        _app.Downloads.Add(download.Id);

        var (response2, authors) =
            await _app.UserA.GETAsync<GetAuthors.Endpoint, GetAuthors.Request, List<DownloadAuthorResponse>>(new()
            {
                DownloadId = download.Id
            });

        response2.StatusCode.Should().Be(HttpStatusCode.OK);

        authors.Should().HaveCount(1);
        authors[0].Id.Should().Be(_app.Users["UserA"].Id);
        authors[0].IsOwner.Should().BeTrue();
    }

    [Fact, Priority(2)]
    public async Task GetAuthors_Single()
    {
        var (response, authors) = await _app.UserA.GETAsync<GetAuthors.Endpoint, GetAuthors.Request, List<DownloadAuthorResponse>>(new()
        {
            DownloadId = _app.Downloads[0]
        });
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        authors.Should().HaveCount(1);
        authors[0].Id.Should().Be(_app.Users["UserA"].Id);
        authors[0].IsOwner.Should().BeTrue();
    }

    [Fact, Priority(3)]
    public async Task GetAuthor_NotFound()
    {
        var response = await _app.UserA.GETAsync<GetAuthor.Endpoint, GetAuthor.Request>(new()
        {
            DownloadId = _app.Downloads[0],
            UserId = Guid.NewGuid()
        });
        
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact, Priority(4)]
    public async Task GetAuthor_ShouldReturnOwner()
    {
        var (response, author) = await _app.UserA.GETAsync<GetAuthor.Endpoint, GetAuthor.Request, DownloadAuthorResponse>(new()
        {
            DownloadId = _app.Downloads[0],
            UserId = _app.Users["UserA"].Id
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        author.Id.Should().Be(_app.Users["UserA"].Id);
        author.IsOwner.Should().BeTrue();
    }

    [Fact, Priority(5)]
    public async Task AddAuthor_DownloadNotFound()
    {
        var response = await _app.UserA.POSTAsync<AddAuthor.Endpoint, AddAuthor.Request>(new()
        {
            DownloadId = Guid.NewGuid().ToString(),
            UserId = _app.Users["UserB"].Id
        });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact, Priority(6)]
    public async Task AddAuthor_Forbidden_WhenUserIsNotOwner()
    {
        var response = await _app.UserB.POSTAsync<AddAuthor.Endpoint, AddAuthor.Request>(new()
        {
            DownloadId = _app.Downloads[0],
            UserId = _app.Users["UserB"].Id
        });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact, Priority(7)]
    public async Task AddAuthor_UserDoesNotExist()
    {
        var (response, error) = await _app.UserA.POSTAsync<AddAuthor.Endpoint, AddAuthor.Request, ErrorResponse>(new()
        {
            DownloadId = _app.Downloads[0],
            UserId = Guid.NewGuid()
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        error.Errors.Should().HaveCount(1);
        error.Errors.Keys.Should().Equal("userId");
    }

    [Fact, Priority(8)]
    public async Task AddAuthor()
    {
        var (response, data) =
            await _app.UserA.POSTAsync<AddAuthor.Endpoint, AddAuthor.Request, DownloadAuthorResponse>(new()
            {
                DownloadId = _app.Downloads[0],
                UserId = _app.Users["UserB"].Id
            });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        data.Id.Should().Be(_app.Users["UserB"].Id);
        data.IsOwner.Should().BeFalse();
    }

    [Fact, Priority(9)]
    public async Task KickAuthor()
    {
        var response = await _app.UserA.DELETEAsync<KickAuthor.Endpoint, KickAuthor.Request>(new()
        {
            DownloadId = _app.Downloads[0],
            UserId = _app.Users["UserB"].Id
        });
        
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}