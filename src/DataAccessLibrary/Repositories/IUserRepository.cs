using TobyMeehan.Com.Data.Repositories.Models;

namespace TobyMeehan.Com.Data.Repositories;

/// <summary>
/// Database repository for users.
/// </summary>
public interface IUserRepository
{
    // SELECT

    Task<List<UserData>> SelectAllAsync();

    Task<List<UserData>> SelectByRoleAsync(string roleId);

    Task<UserData> SelectByIdAsync(string id);

    Task<UserData> SelectByUsernameAsync(string username);
    
    // INSERT

    Task InsertAsync(UserData user);
    
    // UPDATE

    Task UpdateAsync(UserData user);
    
    // DELETE

    Task DeleteAsync(string id);
}