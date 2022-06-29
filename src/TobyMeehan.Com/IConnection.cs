namespace TobyMeehan.Com;

/// <summary>
/// A connection between an application and a user.
/// </summary>
public interface IConnection : IEntity<IConnection>
{
    /// <summary>
    /// The application.
    /// </summary>
    Id<IApplication> ApplicationId { get; }
    
    /// <summary>
    /// The user.
    /// </summary>
    Id<IUser> UserId { get; }
}