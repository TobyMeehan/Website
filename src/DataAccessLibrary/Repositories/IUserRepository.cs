using TobyMeehan.Com.Data.Repositories.Models;

namespace TobyMeehan.Com.Data.Repositories;

/// <summary>
/// Database repository for users.
/// </summary>
public interface IUserRepository
{
    // SELECT

    Task<List<UserData>> SelectAllAsync(CancellationToken cancellationToken = default);

    Task<List<UserData>> SelectByRoleAsync(string roleId, CancellationToken cancellationToken = default);

    Task<UserData> SelectByIdAsync(string id, CancellationToken cancellationToken = default);

    Task<UserData> SelectByHandleAsync(string handle, CancellationToken cancellationToken = default);
    
    // INSERT

    Task InsertAsync(UserData user, CancellationToken cancellationToken = default);
    
    // UPDATE

    Task UpdateAsync(UserData user, CancellationToken cancellationToken = default);
    
    // DELETE

    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
}