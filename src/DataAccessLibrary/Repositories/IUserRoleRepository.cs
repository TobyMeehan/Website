using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Data.Repositories;

public interface IUserRoleRepository
{
    IAsyncEnumerable<RoleDto> SelectAllAsync(LimitStrategy? limit, CancellationToken cancellationToken);
    Task<RoleDto?> SelectByIdAsync(string id, CancellationToken cancellationToken);
    Task<int> InsertAsync(RoleDto data, CancellationToken cancellationToken);
    Task<int> DeleteAsync(string id, CancellationToken cancellationToken);
}