using System.ComponentModel.DataAnnotations;
using TobyMeehan.Com.Domain.Downloads;

namespace TobyMeehan.Com.Data.Models;

public class DownloadDto
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }

    [MaxLength(20)] public string PublicId { get; set; } = null!;
    
    [MaxLength(40)] public string Title { get; set; } = null!;
    [MaxLength(400)] public string Summary { get; set; } = null!;
    [MaxLength(4000)] public string Description { get; set; } = null!;

    public Visibility Visibility { get; set; }
    public Verification Verification { get; set; }

    public string? Version { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public ICollection<DownloadAuthorDto> Authors { get; set; } = [];
    public ICollection<DownloadFileDto> Files { get; set; } = [];
    public ICollection<DownloadCommentDto> Comments { get; set; } = [];
}