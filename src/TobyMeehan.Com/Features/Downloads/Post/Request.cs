using TobyMeehan.Com.Domain.Downloads;

namespace TobyMeehan.Com.Features.Downloads.Post;

public class Request
{
    public string Title { get; set; } = null!;
    public string Summary { get; set; } = null!;
    public string Description { get; set; } = null!;
    public Visibility Visibility { get; set; }
}