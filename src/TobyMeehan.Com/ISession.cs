namespace TobyMeehan.Com;

/// <summary>
/// A session of an application connection on the API.
/// </summary>
public interface ISession : IEntity<ISession>
{
    /// <summary>
    /// The connection (application/user) of the session.
    /// </summary>
    Id<IConnection> ConnectionId { get; }
    
    /// <summary>
    /// The redirect uri used by the application.
    /// </summary>
    Id<IRedirect> RedirectId { get; }
    
    /// <summary>
    /// The authorization code used to create the session.
    /// </summary>
    string AuthorizationCode { get; }
    
    /// <summary>
    /// The scope given to the application for this session.
    /// </summary>
    IEnumerable<string> Scope { get; }
    
    /// <summary>
    /// The PKCE code challenge provided by the application.
    /// </summary>
    string? CodeChallenge { get; }
    
    /// <summary>
    /// The refresh token to be used to create a new session.
    /// </summary>
    string? RefreshToken { get; }
    
    /// <summary>
    /// The time at which the session will expire.
    /// </summary>
    DateTime Expiry { get; }
}