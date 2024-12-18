namespace TobyMeehan.Com.Features.Files.Delete;

public class Request
{
    public string DownloadId { get; set; } = null!;
    public Guid FileId { get; set; }
}