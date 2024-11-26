using System.Security.Claims;
using FastEndpoints;
using TobyMeehan.Com.Domain.Downloads;
using TobyMeehan.Com.Domain.Downloads.Services;

namespace TobyMeehan.Com.Features.Downloads.Post;

public class Endpoint : Endpoint<Request, DownloadResponse, DownloadMapper>
{
    private readonly IDownloadService _downloadService;

    public Endpoint(IDownloadService downloadService)
    {
        _downloadService = downloadService;
    }

    public override void Configure()
    {
        Post("/downloads");
        Policies(Security.Policies.CreateDownload);
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            throw new InvalidOperationException();
        }

        var download = await _downloadService.CreateAsync(new IDownloadService.CreateDownload(
            userId,
            req.Title,
            req.Summary,
            req.Description,
            req.Visibility), ct);

        await SendCreatedAtAsync<GetById.Endpoint>(new { Id = download.Url }, Map.FromEntity(download),
            cancellation: ct);
    }
}