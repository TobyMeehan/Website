namespace TobyMeehan.Com;

/// <summary>
/// A custom download.
/// </summary>
public interface IDownload : IEntity<IDownload>
{
    /// <summary>
    /// The title of the download.
    /// </summary>
    string Title { get; }
    
    /// <summary>
    /// A short description of the download.
    /// </summary>
    string Summary { get; }
    
    /// <summary>
    /// A detailed description of the download.
    /// </summary>
    string? Description { get; }
    
    /// <summary>
    /// The verification status of the download.
    /// </summary>
    string Verification { get; }
    
    /// <summary>
    /// The visibility of the download.
    /// </summary>
    string Visibility { get; }
    
    /// <summary>
    /// The datetime the download was last updated.
    /// </summary>
    DateTime UpdatedAt { get; }
    
    IEntityCollection<IDownloadAuthor, IUser> Authors { get; }
}