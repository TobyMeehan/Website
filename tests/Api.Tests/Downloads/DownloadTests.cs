using System.Net;
using FastEndpoints;
using FastEndpoints.Testing;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TobyMeehan.Com.Data.DataAccess;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Domain.Downloads;
using TobyMeehan.Com.Features.Downloads;
using Create = TobyMeehan.Com.Features.Downloads.Post;
using List = TobyMeehan.Com.Features.Downloads.Get;
using Get = TobyMeehan.Com.Features.Downloads.GetById;
using Update = TobyMeehan.Com.Features.Downloads.Put;
using Delete = TobyMeehan.Com.Features.Downloads.Delete;

namespace Api.Tests.Downloads;

public class DownloadTests(ApiApp App) : TestBase<ApiApp>
{
    [Fact]
    public async Task CreateDownload_InvalidRequest()
    {
        var (response, result) = await App.UserA.POSTAsync<Create.Endpoint, Create.Request, ProblemDetails>(
            new()
            {
                Title = null!,
                Summary = string.Join("", Enumerable.Range(0, 401).Select(_ => 'e')),
                Description = "",
                Visibility = (Visibility) 4
            });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        result.Errors.Should().HaveCount(4);
        result.Errors.Select(x => x.Name).Should().Equal("title", "summary", "description", "visibility");
    }

    [Fact]
    public async Task GetDownload_NotFound()
    {
        var response = await App.Client.GETAsync<Get.Endpoint, Get.Request>(new()
        {
            Id = Guid.NewGuid().ToString()
        });
        
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact, Priority(1)]
    public async Task CreateDownloads()
    {
        for (var i = 0; i < 3; i++)
        {
            var request = new Create.Request
            {
                Title = Fake.Commerce.ProductName(),
                Summary = Fake.Commerce.ProductDescription(),
                Description = Fake.Lorem.Paragraphs(),
                Visibility = Visibility.Public
            };

            var (response, data) =
                await App.UserA.POSTAsync<Create.Endpoint, Create.Request, DownloadResponse>(request);

            response.StatusCode.Should().Be(HttpStatusCode.Created);

            data.Id.Should().NotBeNull();
            data.Title.Should().Be(request.Title);
            data.Summary.Should().Be(request.Summary);
            data.Description.Should().Be(request.Description);
            data.Visibility.Should().Be("public");

            using var scope = App.Services.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var download = await dbContext.Set<DownloadDto>().FirstOrDefaultAsync(x => x.PublicId == data.Id);

            download.Should().NotBeNull();

            download?.Title.Should().Be(request.Title);
            download?.Summary.Should().Be(request.Summary);
            download?.Description.Should().Be(request.Description);
            download?.Visibility.Should().Be(request.Visibility);

            App.Downloads.Add(data.Id);
        }

        App.Downloads.Should().HaveCount(3);
    }

    [Fact, Priority(2)]
    public async Task GetDownloadList()
    {
        var (response, data) = await App.Client.GETAsync<List.Endpoint, List<DownloadResponse>>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        data.Should().HaveCount(3);
        data.Select(x => x.Id).Should().Equal(App.Downloads);
    }

    [Fact, Priority(3)]
    public async Task GetDownload()
    {
        var downloadId = App.Downloads[0];

        var (response, data) = await App.Client.GETAsync<Get.Endpoint, Get.Request, DownloadResponse>(new()
        {
            Id = downloadId
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using var scope = App.Services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var download = await dbContext.Set<DownloadDto>().FirstOrDefaultAsync(x => x.PublicId == data.Id);

        download.Should().NotBeNull();

        download?.Title.Should().Be(data.Title);
        download?.Summary.Should().Be(data.Summary);
        download?.Description.Should().Be(data.Description);
    }

    [Fact, Priority(4)]
    public async Task UpdateDownload_ShouldReturnForbidden_WhenUserIsNotAuthorized()
    {
        var downloadId = App.Downloads[0];

        var request = new Update.Request
        {
            Id = downloadId,
            Title = Fake.Commerce.ProductName(),
            Summary = Fake.Lorem.Paragraph(),
            Description = Fake.Lorem.Paragraphs(),
            Visibility = Visibility.Private,
            Version = Fake.System.Version().ToString()
        };
        
        var response = await App.UserB.PUTAsync<Update.Endpoint, Update.Request>(request);
        
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact, Priority(5)]
    public async Task UpdateDownload_InvalidRequest()
    {
        var downloadId = App.Downloads[0];

        var request = new Update.Request
        {
            Id = downloadId,
            Title = "",
            Summary = null!,
            Description = new string(Enumerable.Range(0, 4001).Select(_ => 'e').ToArray()),
            Visibility = (Visibility) 100,
            Version = Fake.Hacker.Phrase()
        };
        
        var response = await App.UserA.PUTAsync<Update.Endpoint, Update.Request>(request);
        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact, Priority(6)]
    public async Task UpdateDownload()
    {
        var downloadId = App.Downloads[0];

        var request = new Update.Request
        {
            Id = downloadId,
            Title = Fake.Commerce.ProductName(),
            Summary = Fake.Lorem.Paragraph(),
            Description = Fake.Lorem.Paragraphs(),
            Visibility = Visibility.Private,
            Version = Fake.System.Version().ToString()
        };
        
        var (response, data) = await App.UserA.PUTAsync<Update.Endpoint, Update.Request, DownloadResponse>(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        data.Id.Should().Be(request.Id);
        data.Title.Should().Be(request.Title);
        data.Summary.Should().Be(request.Summary);
        data.Description.Should().Be(request.Description);
        data.Visibility.Should().Be("private");
        data.Version.Should().Be(request.Version);
    }

    [Fact, Priority(7)]
    public async Task UpdateDownload_NullVersion()
    {
        var downloadId = App.Downloads[0];

        var request = new Update.Request
        {
            Id = downloadId,
            Title = Fake.Commerce.ProductName(),
            Summary = Fake.Lorem.Paragraph(),
            Description = Fake.Lorem.Paragraphs(),
            Visibility = Visibility.Private,
            Version = null
        };
        
        var (response, data) = await App.UserA.PUTAsync<Update.Endpoint, Update.Request, DownloadResponse>(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        data.Id.Should().Be(request.Id);
        data.Title.Should().Be(request.Title);
        data.Summary.Should().Be(request.Summary);
        data.Description.Should().Be(request.Description);
        data.Visibility.Should().Be("private");
        data.Version.Should().BeNull();
    }

    [Fact, Priority(8)]
    public async Task GetPrivateDownload_ShouldReturnForbidden()
    {
        var downloadId = App.Downloads[0];
        
        var response = await App.Client.GETAsync<Get.Endpoint, Get.Request>(new()
        {
            Id = downloadId
        });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact, Priority(9)]
    public async Task DeleteDownload()
    {
        var downloadId = App.Downloads[0];

        var response = await App.UserA.DELETEAsync<Delete.Endpoint, Delete.Request>(new()
        {
            Id = downloadId
        });

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}