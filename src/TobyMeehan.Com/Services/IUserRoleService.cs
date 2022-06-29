using TobyMeehan.Com.Builders;

namespace TobyMeehan.Com.Services;

/// <summary>
/// Service for user role data.
/// </summary>
public interface IUserRoleService
{
    // GET

    /// <summary>
    /// Gets all roles.
    /// </summary>
    /// <returns></returns>
    Task<IEntityCollection<IUserRole>> GetAllAsync();

    /// <summary>
    /// Gets all roles for the specified user.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    Task<IEntityCollection<IUserRole>> GetByUserAsync(Id<IUser> user);

    /// <summary>
    /// Gets the role with the specified ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<IUserRole> GetByIdAsync(Id<IUserRole> id);

    /// <summary>
    /// Gets the role with the specified name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    Task<IUserRole> GetByNameAsync(string name);
    
    // CREATE

    /// <summary>
    /// Creates a new role with the specified builder.
    /// </summary>
    /// <param name="userRole"></param>
    /// <returns></returns>
    Task<IUserRole> CreateAsync(CreateUserRoleBuilder userRole);
    
    // UPDATE
    
    // DELETE

    /// <summary>
    /// Deletes the specified role.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task DeleteAsync(Id<IUserRole> id);
}