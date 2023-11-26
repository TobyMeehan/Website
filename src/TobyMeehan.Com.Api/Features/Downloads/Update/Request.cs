using TobyMeehan.Com.Api.Requests;

namespace TobyMeehan.Com.Api.Features.Downloads.Update;

public class Request : AuthenticatedRequest
{
    public string DownloadId { get; set; } = default!;
    public Optional<string> Title { get; set; }
    public Optional<string> Summary { get; set; }
    public Optional<string?> Description { get; set; }
    public Optional<string> Visibility { get; set; }
}