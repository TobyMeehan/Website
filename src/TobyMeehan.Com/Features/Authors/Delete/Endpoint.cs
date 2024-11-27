using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using TobyMeehan.Com.Domain.Downloads.Services;

namespace TobyMeehan.Com.Features.Authors.Delete;

public class Endpoint : Endpoint<Request>
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
        Delete("/downloads/{DownloadId}/authors/{UserId}");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var download = await _downloadService.GetByUrlAsync(req.DownloadId, ct);

        if (download is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var authorizationResult =
            await _authorizationService.AuthorizeAsync(User, download, Security.Policies.KickAuthor);

        if (!authorizationResult.Succeeded)
        {
            await SendForbiddenAsync(ct);
            return;
        }

        await _downloadAuthorService.RemoveAsync(download.Id, req.UserId, ct);

        await SendNoContentAsync(ct);
    }
}