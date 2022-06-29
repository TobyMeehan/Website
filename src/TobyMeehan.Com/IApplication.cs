namespace TobyMeehan.Com;

/// <summary>
/// An API application.
/// </summary>
public interface IApplication : IEntity<IApplication>
{
    /// <summary>
    /// The application's author.
    /// </summary>
    Id<IUser> AuthorId { get; }
    
    /// <summary>
    /// The download associated with the application.
    /// </summary>
    Id<IDownload> DownloadId { get; }
    
    /// <summary>
    /// The name of the application.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// The application's description.
    /// </summary>
    string? Description { get; }

    /// <summary>
    /// The application's icon.
    /// </summary>
    IFile? Icon { get; }
    
    /// <summary>
    /// The application's redirect URIs.
    /// </summary>
    IEntityCollection<IRedirect> Redirects { get; }
}