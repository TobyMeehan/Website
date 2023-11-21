using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Data.Repositories;

/// <summary>
/// Database repository for users.
/// </summary>
public interface IUserRepository
{
    IAsyncEnumerable<UserDto> SelectByRoleAsync(string roleId, LimitStrategy? limit, CancellationToken cancellationToken);

    Task<UserDto?> SelectByUsernameAsync(string username, CancellationToken cancellationToken);
    Task<UserDto?> SelectByIdAsync(string id, CancellationToken cancellationToken);
    IAsyncEnumerable<UserDto> SelectAllAsync(LimitStrategy? limit, CancellationToken cancellationToken);
    Task<int> InsertAsync(UserDto user, CancellationToken cancellationToken);
    Task<int> UpdateAsync(string id, UserDto user, CancellationToken cancellationToken);
    Task<int> DeleteAsync(string id, CancellationToken cancellationToken);

    Task<int> AddRoleAsync(string id, string roleId, CancellationToken cancellationToken);
    Task<int> RemoveRoleAsync(string id, string roleId, CancellationToken cancellationToken);
}