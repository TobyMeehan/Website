namespace TobyMeehan.Com.Domain.Downloads;

public class Comment
{
    public required Guid Id { get; init; }
    public required Guid UserId { get; init; }
    public required Guid DownloadId { get; init; }
    
    public required string Content { get; init; }

    public required DateTime CreatedAt { get; init; }
    public required DateTime EditedAt { get; init; }
}