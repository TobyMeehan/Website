using System.Runtime.CompilerServices;
using Microsoft.Extensions.Options;
using OneOf;
using TobyMeehan.Com.Builders.Scope;
using TobyMeehan.Com.Data.Entities;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Data.Services;

public class ConfigurationScopeService : IScopeService
{
    private readonly List<ConfigurationScope> _scopes;

    public ConfigurationScopeService(IOptions<List<ConfigurationScope>> options)
    {
        _scopes = options.Value.Select(x => new ConfigurationScope
        {
            Id = new Id<IScope>(x.Name),
            Name = x.Name,
            Description = x.Description,
            DisplayName = x.DisplayName
        }).ToList();
    }
    
    public async IAsyncEnumerable<IScope> GetAllAsync(QueryOptions? options = null, 
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        foreach (var scope in _scopes)
        {
            yield return scope;
        }
    }

    public Task<OneOf<IScope, NotFound>> GetByIdAsync(Id<IScope> id, QueryOptions? options = null, CancellationToken cancellationToken = default)
    {
        var scope = _scopes.FirstOrDefault(x => x.Id == id);

        if (scope is null)
        {
            return Task.FromResult<OneOf<IScope, NotFound>>(new NotFound());
        }

        return Task.FromResult<OneOf<IScope, NotFound>>(scope);
    }

    public Task<OneOf<IScope, NotFound>> GetByNameAsync(string name, QueryOptions? options = null, CancellationToken cancellationToken = default)
    {
        var scope = _scopes.FirstOrDefault(x => x.Name == name);

        if (scope is null)
        {
            return Task.FromResult<OneOf<IScope, NotFound>>(new NotFound());
        }

        return Task.FromResult<OneOf<IScope, NotFound>>(scope);
    }

    public Task<long> CountAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<long>(_scopes.Count);
    }

    public Task<IScope> CreateAsync(CreateScopeBuilder create, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    public Task<OneOf<IScope, NotFound>> UpdateAsync(UpdateScopeBuilder update, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    public Task<OneOf<Success, NotFound>> DeleteAsync(Id<IScope> id, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }
}