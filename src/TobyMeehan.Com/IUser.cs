namespace TobyMeehan.Com;

/// <summary>
/// A registered user.
/// </summary>
public interface IUser : IEntity<IUser>
{
    /// <summary>
    /// The custom username of the user.
    /// </summary>
    string DisplayName { get; }
    
    /// <summary>
    /// The unique handle of the user.
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
    /// The user's avatar.
    /// </summary>
    IFile? Avatar { get; }
    
    /// <summary>
    /// The user's roles.
    /// </summary>
    IEntityCollection<IUserRole> Roles { get; }
}