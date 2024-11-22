namespace TobyMeehan.Com.Data.Models;

public class DownloadAuthorDto
{
    public Guid DownloadId { get; set; }
    public Guid UserId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DownloadDto? Download { get; set; }
}