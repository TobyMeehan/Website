using System.Collections.Immutable;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.Json;
using OpenIddict.Abstractions;
using TobyMeehan.Com.Accounts.Models;
using TobyMeehan.Com.Accounts.Models.OpenId;

namespace TobyMeehan.Com.Accounts.OpenId;

public abstract class BaseScopeStore : IOpenIddictScopeStore<OpenIdScope>
{
    public abstract ValueTask<long> CountAsync(CancellationToken cancellationToken);

    public abstract ValueTask CreateAsync(OpenIdScope scope, CancellationToken cancellationToken);

    public abstract ValueTask DeleteAsync(OpenIdScope scope, CancellationToken cancellationToken);

    public abstract ValueTask<OpenIdScope?> FindByIdAsync(string identifier, CancellationToken cancellationToken);

    public abstract ValueTask<OpenIdScope?> FindByNameAsync(string name, CancellationToken cancellationToken);

    public virtual async IAsyncEnumerable<OpenIdScope> FindByNamesAsync(ImmutableArray<string> names, 
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        yield break;
    }

    public virtual async IAsyncEnumerable<OpenIdScope> FindByResourceAsync(string resource, 
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        yield break;
    }

    public abstract IAsyncEnumerable<OpenIdScope> ListAsync(int? count, int? offset,
        CancellationToken cancellationToken);
    
    public abstract ValueTask UpdateAsync(OpenIdScope scope, CancellationToken cancellationToken);
    


    public ValueTask<string?> GetDescriptionAsync(OpenIdScope scope, CancellationToken cancellationToken)
        => new(scope.Description);

    public ValueTask<ImmutableDictionary<CultureInfo, string>> GetDescriptionsAsync(OpenIdScope scope,
        CancellationToken cancellationToken)
        => new(ImmutableDictionary.Create<CultureInfo, string>());

    public ValueTask<string?> GetDisplayNameAsync(OpenIdScope scope, CancellationToken cancellationToken)
        => new(scope.DisplayName);

    public ValueTask<ImmutableDictionary<CultureInfo, string>> GetDisplayNamesAsync(OpenIdScope scope,
        CancellationToken cancellationToken)
        => new(ImmutableDictionary.Create<CultureInfo, string>());

    public ValueTask<string?> GetIdAsync(OpenIdScope scope, CancellationToken cancellationToken)
        => new(scope.Id.Value);

    public ValueTask<string?> GetNameAsync(OpenIdScope scope, CancellationToken cancellationToken)
        => new(scope.Name);

    public ValueTask<ImmutableDictionary<string, JsonElement>> GetPropertiesAsync(OpenIdScope scope,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();

    public ValueTask<ImmutableArray<string>> GetResourcesAsync(OpenIdScope scope, CancellationToken cancellationToken)
        => new(ImmutableArray.Create<string>());

    public ValueTask<OpenIdScope> InstantiateAsync(CancellationToken cancellationToken)
        => new(new OpenIdScope());

    
    

    public ValueTask SetDescriptionAsync(OpenIdScope scope, string? description, CancellationToken cancellationToken)
    {
        scope.Description = description;

        return new ValueTask();
    }

    public ValueTask SetDescriptionsAsync(OpenIdScope scope, ImmutableDictionary<CultureInfo, string> descriptions,
        CancellationToken cancellationToken)
        => new();

    public ValueTask SetDisplayNameAsync(OpenIdScope scope, string? name, CancellationToken cancellationToken)
    {
        scope.DisplayName = name;

        return new ValueTask();
    }

    public ValueTask SetDisplayNamesAsync(OpenIdScope scope, ImmutableDictionary<CultureInfo, string> names,
        CancellationToken cancellationToken)
        => new();

    public ValueTask SetNameAsync(OpenIdScope scope, string? name, CancellationToken cancellationToken)
    {
        scope.Name = name;

        return new ValueTask();
    }

    public ValueTask SetPropertiesAsync(OpenIdScope scope, ImmutableDictionary<string, JsonElement> properties,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();

    public ValueTask SetResourcesAsync(OpenIdScope scope, ImmutableArray<string> resources,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();
    
    
    public ValueTask<long> CountAsync<TResult>(Func<IQueryable<OpenIdScope>, IQueryable<TResult>> query,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();
    
    public ValueTask<TResult?> GetAsync<TState, TResult>(
        Func<IQueryable<OpenIdScope>, TState, IQueryable<TResult>> query, TState state,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();
    
    public IAsyncEnumerable<TResult> ListAsync<TState, TResult>(
        Func<IQueryable<OpenIdScope>, TState, IQueryable<TResult>> query, TState state,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();
}