using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using TobyMeehan.Com.Domain.Downloads;
using TobyMeehan.Com.Domain.Downloads.Services;

namespace TobyMeehan.Com.Features.Uploads.Complete;

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
        Post("/downloads/{DownloadId}/files/{FileId}/uploads/{UploadId}/complete");
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
            await _authorizationService.AuthorizeAsync(User, download, Security.Policies.UploadFile);

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

        var parts = req.Parts.Select(x => new FileUploadPart
        {
            PartNumber = x.PartNumber,
            SizeInBytes = x.Size,
            ETag = x.ETag,
        });
        
        await _downloadFileService.CompleteUploadAsync(file, req.UploadId, parts, ct);
    }
}