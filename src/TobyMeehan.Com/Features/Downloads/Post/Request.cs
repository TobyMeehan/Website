using TobyMeehan.Com.Domain.Downloads;

namespace TobyMeehan.Com.Features.Downloads.Post;

public class Request
{
    public string Title { get; set; }
    public string Summary { get; set; }
    public string Description { get; set; }
    public Visibility Visibility { get; set; }
}