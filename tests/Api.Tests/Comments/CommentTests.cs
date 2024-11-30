using System.Net;
using FastEndpoints;
using FastEndpoints.Testing;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using TobyMeehan.Com.Domain.Downloads;
using TobyMeehan.Com.Domain.Downloads.Services;
using TobyMeehan.Com.Features.Comments;
using Create = TobyMeehan.Com.Features.Comments.Post;

namespace Api.Tests.Comments;

public class CommentTests : TestBase<ApiApp>
{
    private readonly ApiApp _app;

    public CommentTests(ApiApp app)
    {
        _app = app;
    }
    
    protected override async Task SetupAsync()
    {
        var downloadService = _app.Services.GetRequiredService<IDownloadService>();

        var download = await downloadService.CreateAsync(new IDownloadService.CreateDownload(
            _app.Users["UserA"].Id,
            Title: Fake.Commerce.ProductName(),
            Summary: Fake.Commerce.ProductDescription(),
            Description: Fake.Lorem.Paragraphs(),
            Visibility: Fake.PickRandom<Visibility>()));
        
        _app.Downloads.Add(download.Url);
    }

    [Fact]
    public async Task CreateComment_Invalid()
    {
        var request = new Create.Request
        {
            DownloadId = _app.Downloads[0],
            Content = ""
        };

        var (response, error) = await _app.UserA.POSTAsync<Create.Endpoint, Create.Request, ErrorResponse>(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        error.Errors.Should().HaveCount(1);
    }

    [Fact]
    public async Task CreateComments()
    {
        for (int i = 0; i < 3; i++)
        {
            var request = new Create.Request
            {
                DownloadId = _app.Downloads[0],
                Content = Fake.Lorem.Paragraph()
            };
        
            var (response, data) = await _app.UserA.POSTAsync<Create.Endpoint, Create.Request, CommentResponse>(request);

            response.StatusCode.Should().Be(HttpStatusCode.Created);
        
            data.Content.Should().Be(request.Content);
        }
    }
}