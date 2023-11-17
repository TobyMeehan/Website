using OneOf;
using TobyMeehan.Com.Models.User;

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
    IAsyncEnumerable<IUser> GetAllAsync(QueryOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the user with the specified ID.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="options"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<OneOf<IUser, NotFound>> GetByIdAsync(Id<IUser> id, QueryOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the user with the specified username.
    /// </summary>
    /// <param name="username"></param>
    /// <param name="options"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<OneOf<IUser, NotFound>> GetByUsernameAsync(string username, QueryOptions? options = null, CancellationToken cancellationToken = default);
    
    Task<OneOf<IUser, InvalidCredentials, NotFound>> GetByCredentialsAsync(string username, Password password, QueryOptions? options = null, CancellationToken cancellationToken = default);

    Task<OneOf<IUser, InvalidCredentials, NotFound>> GetByCredentialsAsync(Id<IUser> id, Password password,
        QueryOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all users with the specified role.
    /// </summary>
    /// <param name="role"></param>
    /// <param name="options"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    IAsyncEnumerable<IUser> GetByRoleAsync(Id<IUserRole> role, QueryOptions? options = null, CancellationToken cancellationToken = default);
    
    Task<bool> IsUsernameUniqueAsync(string username, CancellationToken cancellationToken = default);

    // CREATE

    /// <summary>
    /// Creates a new user with the specified builder.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IUser> CreateAsync(ICreateUser user, CancellationToken cancellationToken = default);
    
    // UPDATE

    /// <summary>
    /// Updates the specified user with the specified builder.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="user"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<OneOf<IUser, NotFound>> UpdateAsync(Id<IUser> id, IUpdateUser user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Changes the specified user's balance by the specified amount.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="amount"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<OneOf<Success, InsufficientBalance, NotFound>> UpdateBalanceAsync(Id<IUser> id, double amount, CancellationToken cancellationToken = default);

    // DELETE

    /// <summary>
    /// Deletes the specified user.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<OneOf<Success, NotFound>> DeleteAsync(Id<IUser> id, CancellationToken cancellationToken = default);
}