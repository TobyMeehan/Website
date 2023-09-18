namespace TobyMeehan.Com;

/// <summary>
/// A session of an application connection on the API.
/// </summary>
public interface ISession : IEntity<ISession>
{
    /// <summary>
    /// The connection (application/user) of the session.
    /// </summary>
    IConnection Connection { get; }

    IApplication Application => Connection.Application;

    IUser User => Connection.User;
    
    /// <summary>
    /// The redirect uri used by the application.
    /// </summary>
    IRedirect? Redirect { get; }
    
    /// <summary>
    /// The scope given to the application for this session.
    /// </summary>
    IEnumerable<string> Scope { get; }
    
    /// <summary>
    /// The refresh token to be used to create a new session.
    /// </summary>
    string? RefreshToken { get; }
    
    /// <summary>
    /// The time at which the session will expire.
    /// </summary>
    DateTime Expiry { get; }
}