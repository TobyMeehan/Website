namespace TobyMeehan.Com.Features.Comments;

public class CommentResponse
{
    public required Guid Id { get; set; }
    public required Guid UserId { get; set; }
    public required string Content { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime? EditedAt { get; set; }
}