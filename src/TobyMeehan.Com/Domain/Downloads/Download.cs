namespace TobyMeehan.Com.Domain.Downloads;

public class Download
{
    public required Guid Id { get; init; }

    public required string PublicId { get; init; }
    public required string Title { get; init; }
    public required string Summary { get; init; }
    public required string Description { get; init; }

    public required Visibility Visibility { get; init; }
    public required Verification Verification { get; init; }

    public required Version? Version { get; init; }

    public required DateTime CreatedAt { get; init; }
    public required DateTime? UpdatedAt { get; init; }
}
