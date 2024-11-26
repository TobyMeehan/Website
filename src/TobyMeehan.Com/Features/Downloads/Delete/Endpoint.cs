using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using TobyMeehan.Com.Domain.Downloads.Services;

namespace TobyMeehan.Com.Features.Downloads.Delete;

public class Endpoint : Endpoint<Request>
{
    private readonly IDownloadService _downloadService;
    private readonly IAuthorizationService _authorizationService;

    public Endpoint(
        IDownloadService downloadService,
        IAuthorizationService authorizationService)
    {
        _downloadService = downloadService;
        _authorizationService = authorizationService;
    }

    public override void Configure()
    {
        Delete("/downloads/{Id}");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var download = await _downloadService.GetByUrlAsync(req.Id, ct);

        if (download is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var authorizationResult =
            await _authorizationService.AuthorizeAsync(User, download, Security.Policies.DeleteDownload);

        if (!authorizationResult.Succeeded)
        {
            await SendForbiddenAsync(ct);
            return;
        }

        await _downloadService.DeleteAsync(download.Id, ct);
    }
}