using OneOf;
using TobyMeehan.Com.Builders;
using TobyMeehan.Com.Builders.Scope;
using TobyMeehan.Com.Models.Scope;
using TobyMeehan.Com.Results;

namespace TobyMeehan.Com.Services;

public interface IScopeService
{
    IAsyncEnumerable<IScope> GetAllAsync(QueryOptions? options = null, CancellationToken cancellationToken = default);
    
    Task<OneOf<IScope, NotFound>> GetByIdAsync(Id<IScope> id, QueryOptions? options = null, CancellationToken cancellationToken = default);

    Task<OneOf<IScope, NotFound>> GetByNameAsync(string name, QueryOptions? options = null, CancellationToken cancellationToken = default);

    Task<OneOf<IScope, NotFound>> GetByAliasAsync(string nameOrAlias, QueryOptions? options = null,
        CancellationToken cancellationToken = default);

    Task<long> CountAsync(CancellationToken cancellationToken = default);
    
    Task<IScope> CreateAsync(ICreateScope create, CancellationToken cancellationToken = default);

    Task<OneOf<IScope, NotFound>> UpdateAsync(Id<IScope> id, IUpdateScope update,
        CancellationToken cancellationToken = default);

    Task<OneOf<Success, NotFound>> DeleteAsync(Id<IScope> id, CancellationToken cancellationToken = default);

    Task<OneOf<Success, Forbidden>> AuthorizeScopeAsync(IScope scope, IUser user, IApplication application,
        CancellationToken cancellationToken = default);
}