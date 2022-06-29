namespace TobyMeehan.Com;

/// <summary>
/// A role for a download.
/// </summary>
public interface IDownloadRole : IRole<IDownloadRole>
{
    /// <summary>
    /// The download the role belongs to.
    /// </summary>
    Id<IDownload> DownloadId { get; }
}