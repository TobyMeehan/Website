using TobyMeehan.Com.Builders;
using TobyMeehan.Com.Builders.Authorization;
using TobyMeehan.Com.Models.Authorization;

namespace TobyMeehan.Com.Services;

public interface IAuthorizationService
{
    IAsyncEnumerable<IAuthorization> FindByApplicationAsync(string applicationId, QueryOptions? options = null,
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<IAuthorization> FindByUserAsync(string userId, QueryOptions? options = null,
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<IAuthorization> FindByApplicationAndUserAsync(string applicationId, string userId,
        QueryOptions? options = null, CancellationToken cancellationToken = default);

    Task<IAuthorization?> FindByIdAsync(string id, QueryOptions? options = null,
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<IAuthorization> GetAllAsync(QueryOptions? options = null,
        CancellationToken cancellationToken = default);

    Task<long> CountAsync(CancellationToken cancellationToken = default);

    Task<IAuthorization> CreateAsync(ICreateAuthorization create, CancellationToken cancellationToken = default);

    Task<IAuthorization> UpdateAsync(Id<IAuthorization> id, IUpdateAuthorization update,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(Id<IAuthorization> id, CancellationToken cancellationToken = default);

    Task DeleteByCreationAsync(DateTime threshold, CancellationToken cancellationToken = default);
}