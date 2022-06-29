namespace TobyMeehan.Com;

/// <summary>
/// A registered user.
/// </summary>
public interface IUser : IEntity<IUser>
{
    /// <summary>
    /// The username of the user.
    /// </summary>
    string Username { get; }
    
    /// <summary>
    /// The balance of the user.
    /// </summary>
    double Balance { get; }
    
    /// <summary>
    /// The user's custom description.
    /// </summary>
    string? Description { get; }
    
    /// <summary>
    /// The user's custom URL.
    /// </summary>
    string? CustomUrl { get; }
    
    /// <summary>
    /// The user's avatar.
    /// </summary>
    IFile? Avatar { get; }
}