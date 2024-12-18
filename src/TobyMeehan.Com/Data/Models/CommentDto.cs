using System.ComponentModel.DataAnnotations;

namespace TobyMeehan.Com.Data.Models;

public class CommentDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    [MaxLength(400)]
    public string Content { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? EditedAt { get; set; }

    public ICollection<ReplyDto> Replies { get; set; } = [];

    public ReplyDto? Parent { get; set; }
    public DownloadCommentDto? Download { get; set; }
}