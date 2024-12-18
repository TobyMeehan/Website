namespace TobyMeehan.Com.Features.Files.Complete;

public class Request
{
    public string DownloadId { get; set; } = null!;
    public Guid FileId { get; set; }
}