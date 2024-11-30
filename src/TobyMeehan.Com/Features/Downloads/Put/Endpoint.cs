using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using TobyMeehan.Com.Domain.Downloads.Services;

namespace TobyMeehan.Com.Features.Downloads.Put;

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
        Put("/downloads/{Id}");
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
            await _authorizationService.AuthorizeAsync(User, download, Security.Policies.EditDownload);

        if (!authorizationResult.Succeeded)
        {
            await SendForbiddenAsync(ct);
            return;
        }

        download = await _downloadService.UpdateAsync(download.Id, new IDownloadService.UpdateDownload(
                req.Title,
                req.Summary,
                req.Description,
                req.Visibility,
                string.IsNullOrEmpty(req.Version) ? null : System.Version.Parse(req.Version)),
            ct);

        Response = Map.FromEntity(download!);
    }
}