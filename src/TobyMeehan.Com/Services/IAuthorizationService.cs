using OneOf;
using TobyMeehan.Com.Builders;
using TobyMeehan.Com.Builders.Authorization;
using TobyMeehan.Com.Models.Authorization;

namespace TobyMeehan.Com.Services;

public interface IAuthorizationService
{
    IAsyncEnumerable<IAuthorization> GetAllAsync(QueryOptions? options = null,
        CancellationToken cancellationToken = default);
    
    Task<OneOf<IAuthorization, NotFound>> GetByIdAsync(Id<IAuthorization> id, QueryOptions? options = null,
        CancellationToken cancellationToken = default);
    
    IAsyncEnumerable<IAuthorization> GetByApplicationAsync(Id<IApplication> application, QueryOptions? options = null,
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<IAuthorization> GetByUserAsync(Id<IUser> user, QueryOptions? options = null,
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<IAuthorization> GetByApplicationAndUserAsync(Id<IApplication> application, Id<IUser> user,
        QueryOptions? options = null, CancellationToken cancellationToken = default);

    Task<long> CountAsync(CancellationToken cancellationToken = default);

    Task<IAuthorization> CreateAsync(ICreateAuthorization create, CancellationToken cancellationToken = default);

    Task<OneOf<IAuthorization, NotFound>> UpdateAsync(Id<IAuthorization> id, IUpdateAuthorization update,
        CancellationToken cancellationToken = default);

    Task<OneOf<Success, NotFound>> DeleteAsync(Id<IAuthorization> id, CancellationToken cancellationToken = default);

    Task DeleteByCreationAsync(DateTime threshold, CancellationToken cancellationToken = default);
}