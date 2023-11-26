using TobyMeehan.Com.Api.Requests;

namespace TobyMeehan.Com.Api.Features.DownloadAuthors.GetByDownload;

public class Request : AuthenticatedRequest
{
    public string DownloadId { get; set; } = default!;
}