using System.Security.Claims;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using TobyMeehan.Com.Domain.Downloads.Services;

namespace TobyMeehan.Com.Features.Files.Download;

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
        Post("/downloads/{DownloadId}/downloads");
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
            await _authorizationService.AuthorizeAsync(User, download, Security.Policies.ViewDownload);

        if (!authorizationResult.Succeeded)
        {
            await SendForbiddenAsync(ct);
            return;
        }

        var file = await _downloadFileService.GetByFilenameAsync(download.Id, req.Filename, ct);

        if (file is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var fileAuthorizationResult =
            await _authorizationService.AuthorizeAsync(User, file, Security.Policies.ViewFile);

        if (!fileAuthorizationResult.Succeeded)
        {
            await SendForbiddenAsync(ct);
            return;
        }

        Guid? userId = Guid.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var guid) ? guid : null;

        var url = await _downloadFileService.CreateDownloadAsync(file, userId, ct);

        Response = new Response
        {
            Id = file.Id,
            DownloadId = download.Url,
            Filename = file.Filename,
            ContentType = file.ContentType,
            Size = file.SizeInBytes,
            CreatedAt = file.CreatedAt,
            Url = url
        };
    }
}