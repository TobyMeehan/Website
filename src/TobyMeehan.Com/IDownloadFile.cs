namespace TobyMeehan.Com;

/// <summary>
/// A file of a download.
/// </summary>
public interface IDownloadFile : IEntity<IDownloadFile>, IFile
{
    /// <summary>
    /// The download.
    /// </summary>
    Id<IDownload> DownloadId { get; }
}