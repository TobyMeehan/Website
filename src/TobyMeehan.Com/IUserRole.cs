namespace TobyMeehan.Com;

/// <summary>
/// A role for a user.
/// </summary>
public interface IUserRole : IEntity<IUserRole>, IRole<IUser>
{
    /// <summary>
    /// The user.
    /// </summary>
    Id<IUser> UserId { get; }
}