using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using TobyMeehan.Com.Domain.Downloads.Services;

namespace TobyMeehan.Com.Features.Downloads.GetById;

public class Endpoint : Endpoint<Request, DownloadResponse, DownloadMapper>
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
        Get("/downloads/{Id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var download = await _downloadService.GetByPublicIdAsync(req.Id, ct);

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

        Response = Map.FromEntity(download);
    }
}