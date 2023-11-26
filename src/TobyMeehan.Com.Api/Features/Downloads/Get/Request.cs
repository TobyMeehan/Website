using TobyMeehan.Com.Api.Requests;

namespace TobyMeehan.Com.Api.Features.Downloads.Get;

public class Request : AuthenticatedRequest
{
    public string DownloadId { get; set; } = default!;
}