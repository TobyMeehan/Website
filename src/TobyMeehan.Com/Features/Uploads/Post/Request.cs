namespace TobyMeehan.Com.Features.Uploads.Post;

public class Request
{
    public string DownloadId { get; set; } = null!;
    public Guid FileId { get; set; }
}