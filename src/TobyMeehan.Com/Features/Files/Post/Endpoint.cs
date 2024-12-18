using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using TobyMeehan.Com.Domain.Downloads.Services;

namespace TobyMeehan.Com.Features.Files.Post;

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
        Post("/downloads/{DownloadId}/files");
        Idempotency();
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

        var request = new CreateDownloadFile(download.Id, req.Filename, req.ContentType, req.Size);

        var result = await _downloadFileService.CreateAsync(request, ct);

        await SendAsync(new Response
        {
            Id = result.File.Id,
            DownloadId = download.PublicId,
            Filename = result.File.Filename,
            ContentType = result.File.ContentType,
            Size = result.File.SizeInBytes,
            CreatedAt = result.File.CreatedAt,
            UploadUrl = result.UploadUrl
        }, 201, ct);
    }
}