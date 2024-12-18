using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using TobyMeehan.Com.Domain.Downloads.Services;

namespace TobyMeehan.Com.Features.Uploads.Delete;

public class Endpoint : Endpoint<UploadRequest>
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
        Delete("/downloads/{DownloadId}/files/{FileId}/uploads/{UploadId}");
    }

    public override async Task HandleAsync(UploadRequest req, CancellationToken ct)
    {
        var download = await _downloadService.GetByPublicIdAsync(req.DownloadId, ct);

        if (download is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var authorizationResult =
            await _authorizationService.AuthorizeAsync(User, download, Security.Policies.ManageDownload);

        if (!authorizationResult.Succeeded)
        {
            await SendForbiddenAsync(ct);
            return;
        }
        
        var file = await _downloadFileService.GetByIdAsync(download.Id, req.FileId, ct);

        if (file is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var fileAuthorizationResult =
            await _authorizationService.AuthorizeAsync(User, file, Security.Policies.DeleteFile);

        if (!fileAuthorizationResult.Succeeded)
        {
            await SendForbiddenAsync(ct);
            return;
        }

        await _downloadFileService.DeleteUploadAsync(file, req.UploadId, ct);
    }
}