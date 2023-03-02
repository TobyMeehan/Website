using TobyMeehan.Com.Data.Repositories.Models;

namespace TobyMeehan.Com.Data.Repositories;

/// <summary>
/// Database repository for users.
/// </summary>
public interface IUserRepository : IRepository<UserData>
{
    Task<List<UserData>> SelectByRoleAsync(string roleId, CancellationToken cancellationToken);

    Task<UserData?> SelectByHandleAsync(string handle, CancellationToken cancellationToken);
}