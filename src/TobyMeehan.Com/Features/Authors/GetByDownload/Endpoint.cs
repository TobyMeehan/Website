using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using TobyMeehan.Com.Domain.Downloads.Services;

namespace TobyMeehan.Com.Features.Authors.GetByDownload;

public class Endpoint : Endpoint<Request, List<DownloadAuthorResponse>, DownloadAuthorMapper>
{
    private readonly IDownloadService _downloadService;
    private readonly IDownloadAuthorService _downloadAuthorService;
    private readonly IAuthorizationService _authorizationService;

    public Endpoint(
        IDownloadService downloadService,
        IDownloadAuthorService downloadAuthorService,
        IAuthorizationService authorizationService)
    {
        _downloadService = downloadService;
        _downloadAuthorService = downloadAuthorService;
        _authorizationService = authorizationService;
    }

    public override void Configure()
    {
        Get("/downloads/{DownloadId}/authors");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var download = await _downloadService.GetByPublicIdAsync(req.DownloadId, ct);

        if (download is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var authorizationResult =
            await _authorizationService.AuthorizeAsync(User, download, Security.Policies.ViewDownload);

        if (!authorizationResult.Succeeded)
        {
            await SendForbiddenAsync(ct);
            return;
        }

        var authors = await _downloadAuthorService.GetByDownloadAsync(download.Id, ct);

        Response = authors.Select(Map.FromEntity).ToList();
    }
}