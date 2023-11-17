using OneOf;
using TobyMeehan.Com.Builders;
using TobyMeehan.Com.Builders.Scope;

namespace TobyMeehan.Com.Services;

public interface IScopeService
{
    IAsyncEnumerable<IScope> GetAllAsync(QueryOptions? options = null, CancellationToken cancellationToken = default);
    
    Task<OneOf<IScope, NotFound>> GetByIdAsync(Id<IScope> id, QueryOptions? options = null, CancellationToken cancellationToken = default);

    Task<OneOf<IScope, NotFound>> GetByNameAsync(string name, QueryOptions? options = null, CancellationToken cancellationToken = default);

    Task<long> CountAsync(CancellationToken cancellationToken = default);
    
    Task<IScope> CreateAsync(CreateScopeBuilder create, CancellationToken cancellationToken = default);

    Task<OneOf<IScope, NotFound>> UpdateAsync(UpdateScopeBuilder update, CancellationToken cancellationToken = default);

    Task<OneOf<Success, NotFound>> DeleteAsync(Id<IScope> id, CancellationToken cancellationToken = default);
}