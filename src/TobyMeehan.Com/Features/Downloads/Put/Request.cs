using TobyMeehan.Com.Domain.Downloads;

namespace TobyMeehan.Com.Features.Downloads.Put;

public class Request
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Summary { get; set; }
    public string Description { get; set; }
    public Visibility Visibility { get; set; }
    public string Version { get; set; }
}