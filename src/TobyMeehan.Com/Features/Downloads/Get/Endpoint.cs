using FastEndpoints;
using TobyMeehan.Com.Domain.Downloads.Services;

namespace TobyMeehan.Com.Features.Downloads.Get;

public class Endpoint : EndpointWithoutRequest<List<DownloadResponse>, DownloadMapper>
{
    private readonly IDownloadService _downloadService;

    public Endpoint(IDownloadService downloadService)
    {
        _downloadService = downloadService;
    }
    
    public override void Configure()
    {
        Get("/downloads");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var downloads = await _downloadService.GetPublicAsync(ct);
        
        Response = downloads.Select(Map.FromEntity).ToList();
    }
}