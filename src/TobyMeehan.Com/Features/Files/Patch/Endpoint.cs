using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using TobyMeehan.Com.Domain.Downloads.Services;

namespace TobyMeehan.Com.Features.Files.Patch;

public class Endpoint : Endpoint<Request, DownloadFileResponse>
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
        Patch("/downloads/{DownloadId}/files/{FileId}");
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
            await _authorizationService.AuthorizeAsync(User, file, Security.Policies.EditFile);

        if (!fileAuthorizationResult.Succeeded)
        {
            await SendForbiddenAsync(ct);
            return;
        }

        var update = new UpdateDownloadFile(req.Filename);
        
        await _downloadFileService.UpdateAsync(download.Id, file.Id, update, ct);

        Response = new DownloadFileResponse
        {
            Id = file.Id,
            DownloadId = download.PublicId,
            Filename = update.Filename,
            ContentType = file.ContentType,
            Size = file.SizeInBytes,
            CreatedAt = file.CreatedAt
        };
    }
}