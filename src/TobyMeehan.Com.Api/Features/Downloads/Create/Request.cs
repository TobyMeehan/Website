using TobyMeehan.Com.Api.Requests;

namespace TobyMeehan.Com.Api.Features.Downloads.Create;

public class Request : AuthenticatedRequest
{
    public string Title { get; set; } = default!;
    public string Summary { get; set; } = default!;
    public Optional<string?> Description { get; set; }
    public Optional<string> Visibility { get; set; }
}