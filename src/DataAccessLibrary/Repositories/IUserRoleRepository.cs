using TobyMeehan.Com.Data.Repositories.Models;

namespace TobyMeehan.Com.Data.Repositories;

/// <summary>
/// Database repository for user roles.
/// </summary>
public interface IUserRoleRepository
{
    // SELECT

    /// <summary>
    /// Selects all roles.
    /// </summary>
    /// <returns></returns>
    Task<List<UserRoleData>> SelectAllAsync();

    /// <summary>
    /// Selects the roles of the specified user.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<List<UserRoleData>> SelectByUserAsync(string userId);

    /// <summary>
    /// Selects the role with the specified ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<UserRoleData> SelectByIdAsync(string id);

    /// <summary>
    /// Selects the role with the specified name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    Task<UserRoleData> SelectByNameAsync(string name);
    
    // INSERT
    
    /// <summary>
    /// Inserts the specified role data.
    /// </summary>
    /// <param name="role"></param>
    /// <returns></returns>
    Task InsertAsync(UserRoleData role);
    
    // DELETE

    /// <summary>
    /// Deletes the specified role.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task DeleteAsync(string id);
}