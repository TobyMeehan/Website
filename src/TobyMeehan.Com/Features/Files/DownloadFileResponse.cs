namespace TobyMeehan.Com.Features.Files;

public class DownloadFileResponse
{
    public required Guid Id { get; set; }
    public required string DownloadId { get; set; }
    public required string Filename { get; set; }
    public required string ContentType { get; set; }
    public required long Size { get; set; }
    public required DateTime CreatedAt { get; set; }
}