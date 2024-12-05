using System.ComponentModel.DataAnnotations;
using TobyMeehan.Com.Domain.Downloads;

namespace TobyMeehan.Com.Data.Models;

public class DownloadFileDto
{
    public Guid Id { get; set; }
    public Guid DownloadId { get; set; }

    [MaxLength(40)]
    public string Filename { get; set; } = null!;
    [MaxLength(40)]
    public string ContentType { get; set; } = null!;
    public long SizeInBytes { get; set; }

    public Visibility Visibility { get; set; }
    public FileStatus Status { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public DownloadDto? Download { get; set; }
    public ICollection<FileDownloadDto> Downloads { get; set; }
}