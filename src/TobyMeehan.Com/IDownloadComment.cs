namespace TobyMeehan.Com;

/// <summary>
/// A comment on a download.
/// </summary>
public interface IDownloadComment : IComment<IDownloadComment>
{
    /// <summary>
    /// The download.
    /// </summary>
    Id<IDownload> DownloadId { get; }
}