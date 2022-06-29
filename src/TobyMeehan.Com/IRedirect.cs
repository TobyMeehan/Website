namespace TobyMeehan.Com;

/// <summary>
/// A redirect URI for an application.
/// </summary>
public interface IRedirect : IEntity<IRedirect>
{
    /// <summary>
    /// The application.
    /// </summary>
    Id<IApplication> ApplicationId { get; }
    
    /// <summary>
    /// The URI.
    /// </summary>
    Uri Uri { get; }
}