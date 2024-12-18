using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using TobyMeehan.Com.Domain.Downloads;
using TobyMeehan.Com.Domain.Downloads.Services;

namespace TobyMeehan.Com.Features.Uploads.Post;

public class Endpoint : Endpoint<Request, Response>
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
        Post("/downloads/{DownloadId}/files/{FileId}/uploads");
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

        var upload = await _downloadFileService.CreateUploadAsync(file, ct);

        if (upload is null)
        {
            await SendErrorsAsync(cancellation: ct);
            return;
        }
        
        Response = new Response
        {
            Id = upload.Id
        };

        await SendAsync(Response, 201, ct);
    }
}