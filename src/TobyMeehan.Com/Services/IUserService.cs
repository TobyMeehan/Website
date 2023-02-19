using TobyMeehan.Com.Builders;

namespace TobyMeehan.Com.Services;

/// <summary>
/// Service for user data.
/// </summary>
public interface IUserService
{
    // GET

    /// <summary>
    /// Gets all users.
    /// </summary>
    /// <returns></returns>
    Task<IEntityCollection<IUser>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all users with the specified role.
    /// </summary>
    /// <param name="role"></param>
    /// <returns></returns>
    Task<IEntityCollection<IUser>> GetByRoleAsync(Id<IUserRole> role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the user with the specified ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<IUser> GetByIdAsync(Id<IUser> id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the user with the specified username.
    /// </summary>
    /// <param name="handle"></param>
    /// <returns></returns>
    Task<IUser> GetByHandleAsync(string handle, CancellationToken cancellationToken = default);

    // CREATE

    /// <summary>
    /// Creates a new user with the specified builder.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    Task<IUser> CreateAsync(CreateUserBuilder user, CancellationToken cancellationToken = default);
    
    // UPDATE
    
    /// <summary>
    /// Updates the specified user with the specified builder.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    Task<IUser> UpdateAsync(Id<IUser> id, UpdateUserBuilder user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the protected properties of the specified user with the specified builder.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    Task<IUser> ProtectedUpdateAsync(Id<IUser> id, ProtectedUpdateUserBuilder user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Changes the specified user's balance by the specified amount.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="amount"></param>
    /// <returns></returns>
    Task UpdateBalanceAsync(Id<IUser> id, double amount, CancellationToken cancellationToken = default);

    // DELETE
    
    /// <summary>
    /// Deletes the specified user.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task DeleteAsync(Id<IUser> id, CancellationToken cancellationToken = default);
}