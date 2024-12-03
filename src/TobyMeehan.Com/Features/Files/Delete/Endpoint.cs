using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using TobyMeehan.Com.Domain.Downloads.Services;

namespace TobyMeehan.Com.Features.Files.Delete;

public class Endpoint : Endpoint<Request>
{
    private readonly IDownloadService _downloadService;
    private readonly IDownloadFileService _downloadFileService;
    private readonly IAuthorizationService _authorizationService;

    public Endpoint(
        IDownloadService downloadService,
        IDownloadFileService downloadFileService,
        IAuthorizationService authorizationService)
    {
        _downloadService = downloadService;
        _downloadFileService = downloadFileService;
        _authorizationService = authorizationService;
    }

    public override void Configure()
    {
        Delete("/downloads/{DownloadId}/files/{FileId}");
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
            await _authorizationService.AuthorizeAsync(User, download, Security.Policies.DeleteFile);

        if (!authorizationResult.Succeeded)
        {
            await SendForbiddenAsync(ct);
            return;
        }

        await _downloadFileService.DeleteAsync(download.Id, req.FileId, ct);

        await SendNoContentAsync(ct);
    }
}