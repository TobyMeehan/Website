namespace TobyMeehan.Com;

/// <summary>
/// A custom download.
/// </summary>
public interface IDownload : IEntity<IDownload>
{
    /// <summary>
    /// The owner of the download.
    /// </summary>
    Id<IUser> OwnerId { get; }

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
    string Description { get; }
    
    /// <summary>
    /// The visibility of the download.
    /// </summary>
    Visibility Visibility { get; }
    
    /// <summary>
    /// The download's version.
    /// </summary>
    Version Version { get; }
    
    /// <summary>
    /// The datetime (if any) the download was updated.
    /// </summary>
    DateTimeOffset? UpdatedAt { get; }
}