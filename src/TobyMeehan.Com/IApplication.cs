namespace TobyMeehan.Com;

/// <summary>
/// An application.
/// </summary>
public interface IApplication : IEntity<IApplication>
{
    /// <summary>
    /// The author of the application.
    /// </summary>
    Id<IUser> AuthorId { get; }
    
    /// <summary>
    /// The download associated with the application.
    /// </summary>
    Id<IDownload>? DownloadId { get; }
    
    /// <summary>
    /// The name of the application.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// The description of the application.
    /// </summary>
    string? Description { get; }

    /// <summary>
    /// The icon image of the application.
    /// </summary>
    IFile? Icon { get; }
    
    /// <summary>
    /// The redirects registered by the application.
    /// </summary>
    IEntityCollection<IRedirect> Redirects { get; }
}