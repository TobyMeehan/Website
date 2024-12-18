using TobyMeehan.Com.Domain.Downloads;

namespace TobyMeehan.Com.Features.Downloads.Put;

public class Request
{
    public string Id { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Summary { get; set; } = null!;
    public string Description { get; set; } = null!;
    public Visibility Visibility { get; set; }
    public string? Version { get; set; }
}