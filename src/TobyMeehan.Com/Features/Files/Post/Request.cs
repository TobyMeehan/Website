namespace TobyMeehan.Com.Features.Files.Post;

public class Request
{
    public string DownloadId { get; set; }
    public string Filename { get; set; }
    public string ContentType { get; set; }
    public long Size { get; set; }
}