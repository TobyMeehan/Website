namespace TobyMeehan.Com.Data.Domain.Downloads.Models;

public class Author : IDownloadAuthor
{
    public Id<IUser> Id { get; init; }
    public required Id<IDownload> DownloadId { get; init; }
    public required string? Username { get; init; }
    public required string? DisplayName { get; init; }
    public required bool CanEdit { get; init; }
    public required bool CanManageAuthors { get; init; }
    public required bool CanManageFiles { get; init; }
    public required bool CanDelete { get; init; }
}