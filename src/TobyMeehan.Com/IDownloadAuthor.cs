namespace TobyMeehan.Com;

/// <summary>
/// An author of a download.
/// </summary>
public interface IDownloadAuthor : IEntity<IUser>
{
    /// <summary>
    /// The download.
    /// </summary>
    Id<IDownload> DownloadId { get; }

    string? Username { get; }
    string? DisplayName { get; }
    
    bool CanEdit { get; }
    bool CanManageAuthors { get; }
    bool CanManageFiles { get; }
    bool CanDelete { get; }
}