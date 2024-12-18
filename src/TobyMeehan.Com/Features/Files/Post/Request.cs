namespace TobyMeehan.Com.Features.Files.Post;

public class Request
{
    public string DownloadId { get; set; } = null!;
    public string Filename { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public long Size { get; set; }
}