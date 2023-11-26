namespace TobyMeehan.Com.Data.Domain.Downloads.Models;

public class Download : IDownload
{
    public required Id<IDownload> Id { get; init; }
    public required string Title { get; init; }
    public required string Summary { get; init; }
    public required string? Description { get; init; }
    public required string Verification { get; init; }
    public required string Visibility { get; init; }
    public required DateTime UpdatedAt { get; init; }
    public required IEntityCollection<IDownloadAuthor, IUser> Authors { get; init; }
}