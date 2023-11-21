using OneOf;
using TobyMeehan.Com.Models.Role;
using TobyMeehan.Com.Results;

namespace TobyMeehan.Com.Services;

public interface IUserRoleService
{
    IAsyncEnumerable<IUserRole> GetAllAsync(QueryOptions? queryOptions = null, CancellationToken cancellationToken = default);

    Task<OneOf<IUserRole, NotFound>> GetByIdAsync(Id<IUserRole> id, CancellationToken cancellationToken = default);
    
    Task<IUserRole> CreateAsync(ICreateRole role, CancellationToken cancellationToken = default);

    Task<OneOf<Success, NotFound>> DeleteAsync(Id<IUserRole> id, CancellationToken cancellationToken = default);
}