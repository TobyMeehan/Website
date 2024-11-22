namespace TobyMeehan.Com.Domain.Downloads;

public class DownloadFile
{
    public required Guid Id { get; init; }
    public required Guid DownloadId { get; init; }

    public required string Filename { get; init; }
    public required string ContentType { get; init; }
    public required long SizeInBytes { get; init; }

    public required Visibility Visibility { get; init; }

    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
}