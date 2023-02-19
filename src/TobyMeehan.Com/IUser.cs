namespace TobyMeehan.Com;

/// <summary>
/// A registered user.
/// </summary>
public interface IUser : IEntity<IUser>
{
    /// <summary>
    /// The custom username of the user.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// The unique handle of the user.
    /// </summary>
    string Handle { get; }
    
    /// <summary>
    /// The balance of the user.
    /// </summary>
    double Balance { get; }
    
    /// <summary>
    /// The user's custom description.
    /// </summary>
    string? Description { get; }
    
    /// <summary>
    /// The user's avatar.
    /// </summary>
    IFile? Avatar { get; }
}