using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using TobyMeehan.Com.Domain.Downloads.Services;

namespace TobyMeehan.Com.Features.Files.Get;

public class Endpoint : Endpoint<Request, List<DownloadFileResponse>>
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
        Get("/downloads/{DownloadId}/files");
        AllowAnonymous();
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
            await _authorizationService.AuthorizeAsync(User, download, Security.Policies.ViewDownload);

        if (!authorizationResult.Succeeded)
        {
            await SendForbiddenAsync(ct);
            return;
        }

        var files = await _downloadFileService.GetByDownloadAsync(download.Id, ct);

        Response = files.Select(file => new DownloadFileResponse
        {
            Id = file.Id,
            DownloadId = download.PublicId,
            Filename = file.Filename,
            ContentType = file.ContentType,
            Size = file.SizeInBytes,
            CreatedAt = file.CreatedAt
        }).ToList();
    }
}