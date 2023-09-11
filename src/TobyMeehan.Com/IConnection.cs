namespace TobyMeehan.Com;

/// <summary>
/// A connection between an application and a user.
/// </summary>
public interface IConnection : IEntity<IConnection>
{
    /// <summary>
    /// The application.
    /// </summary>
    IApplication Application { get; }
    
    /// <summary>
    /// The user.
    /// </summary>
    IUser User { get; }
    
    /// <summary>
    /// Whether future authorization requests should be approved automatically.
    /// </summary>
    bool AutoAuthorize { get; }
}