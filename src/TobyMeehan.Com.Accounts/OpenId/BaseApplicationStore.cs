using System.Collections.Immutable;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.Json;
using OpenIddict.Abstractions;
using TobyMeehan.Com.Accounts.Models;
using TobyMeehan.Com.Accounts.Models.OpenId;
using TobyMeehan.Com.Builders;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Accounts.OpenId;

public abstract class BaseApplicationStore : IOpenIddictApplicationStore<OpenIdApplication>
{
    public abstract ValueTask<long> CountAsync(CancellationToken cancellationToken);

    public virtual ValueTask CreateAsync(OpenIdApplication application, CancellationToken cancellationToken)
        => throw new NotSupportedException();
    
    public virtual ValueTask UpdateAsync(OpenIdApplication application, CancellationToken cancellationToken)
        => throw new NotSupportedException();

    public virtual ValueTask DeleteAsync(OpenIdApplication application, CancellationToken cancellationToken)
        => throw new NotSupportedException();

    public abstract ValueTask<OpenIdApplication?> FindByIdAsync(string identifier, CancellationToken cancellationToken);

    public abstract ValueTask<OpenIdApplication?> FindByClientIdAsync(string identifier, CancellationToken cancellationToken);

    public virtual async IAsyncEnumerable<OpenIdApplication> FindByPostLogoutRedirectUriAsync(string uri,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        yield break;
    }

    public abstract IAsyncEnumerable<OpenIdApplication> FindByRedirectUriAsync(string uri, CancellationToken cancellationToken);

    public virtual ValueTask<string?> GetClientIdAsync(OpenIdApplication application, CancellationToken cancellationToken) =>
        new(application.Id.Value);

    public virtual ValueTask<string?> GetClientSecretAsync(OpenIdApplication application, CancellationToken cancellationToken)
        => throw new NotSupportedException();

    public virtual ValueTask<string?> GetClientTypeAsync(OpenIdApplication application,
        CancellationToken cancellationToken)
        => new(application.ClientType);

    public virtual ValueTask<string?> GetConsentTypeAsync(OpenIdApplication application,
        CancellationToken cancellationToken)
        => new(OpenIddictConstants.ConsentTypes.Explicit);

    public virtual ValueTask<string?> GetDisplayNameAsync(OpenIdApplication application, CancellationToken cancellationToken)
        => new(application.Name);

    public virtual ValueTask<ImmutableDictionary<CultureInfo, string>> GetDisplayNamesAsync(OpenIdApplication application,
        CancellationToken cancellationToken)
        => new(ImmutableDictionary.Create<CultureInfo, string>());

    public virtual ValueTask<string?> GetIdAsync(OpenIdApplication application, CancellationToken cancellationToken)
        => new(application.Id.Value);

    public abstract ValueTask<ImmutableArray<string>> GetPermissionsAsync(OpenIdApplication application,
        CancellationToken cancellationToken);

    public virtual ValueTask<ImmutableArray<string>> GetPostLogoutRedirectUrisAsync(OpenIdApplication application,
        CancellationToken cancellationToken)
        => new(ImmutableArray.Create<string>());

    public ValueTask<ImmutableDictionary<string, JsonElement>> GetPropertiesAsync(OpenIdApplication application,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();

    public abstract ValueTask<ImmutableArray<string>> GetRedirectUrisAsync(OpenIdApplication application,
        CancellationToken cancellationToken);

    public abstract ValueTask<ImmutableArray<string>> GetRequirementsAsync(OpenIdApplication application,
        CancellationToken cancellationToken);

    public ValueTask<OpenIdApplication> InstantiateAsync(CancellationToken cancellationToken)
        => throw new NotSupportedException();

    public abstract IAsyncEnumerable<OpenIdApplication> ListAsync(int? count, int? offset,
        CancellationToken cancellationToken);

    public ValueTask SetClientIdAsync(OpenIdApplication application, string? identifier,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();

    public ValueTask SetClientSecretAsync(OpenIdApplication application, string? secret,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();

    public ValueTask SetClientTypeAsync(OpenIdApplication application, string? type,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();

    public ValueTask SetConsentTypeAsync(OpenIdApplication application, string? type,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();

    public ValueTask SetDisplayNameAsync(OpenIdApplication application, string? name,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();

    public ValueTask SetDisplayNamesAsync(OpenIdApplication application, ImmutableDictionary<CultureInfo, string> names,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();

    public ValueTask SetPermissionsAsync(OpenIdApplication application, ImmutableArray<string> permissions,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();

    public ValueTask SetPostLogoutRedirectUrisAsync(OpenIdApplication application, ImmutableArray<string> uris,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();

    public ValueTask SetPropertiesAsync(OpenIdApplication application,
        ImmutableDictionary<string, JsonElement> properties,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();

    public ValueTask SetRedirectUrisAsync(OpenIdApplication application, ImmutableArray<string> uris,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();

    public ValueTask SetRequirementsAsync(OpenIdApplication application, ImmutableArray<string> requirements,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();

    public ValueTask<long> CountAsync<TResult>(
        Func<IQueryable<OpenIdApplication>, IQueryable<TResult>> query,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();
    
    public ValueTask<TResult?> GetAsync<TState, TResult>(
        Func<IQueryable<OpenIdApplication>, TState, IQueryable<TResult>> query, TState state,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();
    
    public IAsyncEnumerable<TResult> ListAsync<TState, TResult>(
        Func<IQueryable<OpenIdApplication>, TState, IQueryable<TResult>> query, TState state,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();
}