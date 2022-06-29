namespace TobyMeehan.Com;

/// <summary>
/// An author of a download.
/// </summary>
public interface IDownloadAuthor : IUser
{
    /// <summary>
    /// The download.
    /// </summary>
    Id<IDownload> DownloadId { get; }
}