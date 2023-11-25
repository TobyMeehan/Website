using TobyMeehan.Com.Data.Domain.Scopes.Models;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Data.Domain.Scopes.Repositories;

public interface IScopeRepository
{
    IAsyncEnumerable<ScopeDto> SelectAllAsync(LimitStrategy? limit, CancellationToken cancellationToken);
    Task<ScopeDto?> SelectByIdAsync(string id, CancellationToken cancellationToken);
    Task<ScopeDto?> SelectByNameAsync(string name, CancellationToken cancellationToken);
    Task<ScopeDto?> SelectByAliasAsync(string alias, CancellationToken cancellationToken);
    Task<long> CountAsync(CancellationToken cancellationToken);
    Task<int> InsertAsync(ScopeDto data, CancellationToken cancellationToken);
    Task<int> UpdateAsync(string id, ScopeDto data, CancellationToken cancellationToken);
    Task<int> DeleteAsync(string id, CancellationToken cancellationToken);
}