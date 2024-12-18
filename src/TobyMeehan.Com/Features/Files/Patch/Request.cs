namespace TobyMeehan.Com.Features.Files.Patch;

public class Request
{
    public string DownloadId { get; set; } = null!;
    public Guid FileId { get; set; }
    public string Filename { get; set; } = null!;
}