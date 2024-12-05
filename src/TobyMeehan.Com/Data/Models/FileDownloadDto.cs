namespace TobyMeehan.Com.Data.Models;

public class FileDownloadDto
{
    public Guid Id { get; set; }
    public Guid FileId { get; set; }
    public Guid? UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DownloadFileDto? File { get; set; } = null!;
}