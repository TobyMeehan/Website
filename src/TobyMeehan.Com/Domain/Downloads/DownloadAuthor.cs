namespace TobyMeehan.Com.Domain.Downloads;

public class DownloadAuthor
{
    public required Guid UserId { get; init; }
    public required Guid DownloadId { get; init; }

    public required bool IsOwner { get; init; }

    public required DateTime CreatedAt { get; init; }
}