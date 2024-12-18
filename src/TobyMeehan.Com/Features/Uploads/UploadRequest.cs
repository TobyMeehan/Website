namespace TobyMeehan.Com.Features.Uploads;

public class UploadRequest
{
    public string DownloadId { get; set; } = null!;
    public Guid FileId { get; set; }
    public string UploadId { get; set; } = null!;
}