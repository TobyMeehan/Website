using TobyMeehan.Com.Builders;
using TobyMeehan.Com.Builders.Scope;

namespace TobyMeehan.Com.Services;

public interface IScopeService
{
    Task<IScope?> FindByIdAsync(string id, QueryOptions? options = null, CancellationToken cancellationToken = default);

    Task<IScope?> FindByNameAsync(string name, QueryOptions? options = null, CancellationToken cancellationToken = default);

    IAsyncEnumerable<IScope> GetAllAsync(QueryOptions? options = null, CancellationToken cancellationToken = default);

    Task<long> CountAsync(CancellationToken cancellationToken = default);
    
    Task<IScope> CreateAsync(CreateScopeBuilder create, CancellationToken cancellationToken = default);

    Task<IScope> UpdateAsync(UpdateScopeBuilder update, CancellationToken cancellationToken = default);

    Task DeleteAsync(Id<IScope> id, CancellationToken cancellationToken = default);
}