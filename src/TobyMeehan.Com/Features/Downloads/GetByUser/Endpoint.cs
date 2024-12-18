using System.Security.Claims;
using FastEndpoints;
using TobyMeehan.Com.Domain.Downloads.Services;

namespace TobyMeehan.Com.Features.Downloads.GetByUser;

public class Endpoint : EndpointWithoutRequest<List<DownloadResponse>, DownloadMapper>
{
    private readonly IDownloadService _downloadService;

    public Endpoint(IDownloadService downloadService)
    {
        _downloadService = downloadService;
    }
    
    public override void Configure()
    {
        Get("/authors/@me/downloads");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            throw new InvalidOperationException();
        }
        
        var downloads = await _downloadService.GetByUserAsync(userId, ct);

        Response = downloads.Select(Map.FromEntity).ToList();
    }
}