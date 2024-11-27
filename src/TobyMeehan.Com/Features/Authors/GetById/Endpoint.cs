using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using TobyMeehan.Com.Domain.Downloads.Services;

namespace TobyMeehan.Com.Features.Authors.GetById;

public class Endpoint : Endpoint<Request, DownloadAuthorResponse, DownloadAuthorMapper>
{
    private readonly IDownloadService _downloadService;
    private readonly IDownloadAuthorService _authorService;
    private readonly IAuthorizationService _authorizationService;

    public Endpoint(
        IDownloadService downloadService,
        IDownloadAuthorService authorService,
        IAuthorizationService authorizationService)
    {
        _downloadService = downloadService;
        _authorService = authorService;
        _authorizationService = authorizationService;
    }

    public override void Configure()
    {
        Get("/downloads/{DownloadId}/authors/{UserId}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var download = await _downloadService.GetByUrlAsync(req.DownloadId, ct);

        if (download is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }
        
        var authorizationResult = await _authorizationService.AuthorizeAsync(User, download, Security.Policies.ViewDownload);

        if (!authorizationResult.Succeeded)
        {
            await SendForbiddenAsync(ct);
            return;
        }
        
        var authors = await _authorService.GetByDownloadAsync(download.Id, ct);

        var author = authors.FirstOrDefault(x => x.UserId == req.UserId);

        if (author is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }
        
        Response = Map.FromEntity(author);
    }
}