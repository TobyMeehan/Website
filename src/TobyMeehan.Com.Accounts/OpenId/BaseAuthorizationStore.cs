using System.Collections.Immutable;
using System.Text.Json;
using OpenIddict.Abstractions;
using TobyMeehan.Com.Accounts.Models;
using TobyMeehan.Com.Accounts.Models.OpenId;

namespace TobyMeehan.Com.Accounts.OpenId;

public abstract class BaseAuthorizationStore : IOpenIddictAuthorizationStore<OpenIdAuthorization>
{
    public abstract ValueTask<long> CountAsync(CancellationToken cancellationToken);
    
    public abstract ValueTask CreateAsync(OpenIdAuthorization authorization, CancellationToken cancellationToken);

    public abstract ValueTask DeleteAsync(OpenIdAuthorization authorization, CancellationToken cancellationToken);

    public abstract IAsyncEnumerable<OpenIdAuthorization> FindAsync(string subject, string client,
        CancellationToken cancellationToken);

    public abstract IAsyncEnumerable<OpenIdAuthorization> FindAsync(string subject, string client, string status,
        CancellationToken cancellationToken);

    public abstract IAsyncEnumerable<OpenIdAuthorization> FindAsync(string subject, string client, string status,
        string type, CancellationToken cancellationToken);

    public abstract IAsyncEnumerable<OpenIdAuthorization> FindAsync(string subject, string client, string status,
        string type, ImmutableArray<string> scopes, CancellationToken cancellationToken);

    public abstract IAsyncEnumerable<OpenIdAuthorization> FindByApplicationIdAsync(string identifier,
        CancellationToken cancellationToken);

    public abstract ValueTask<OpenIdAuthorization?> FindByIdAsync(string identifier,
        CancellationToken cancellationToken);

    public abstract IAsyncEnumerable<OpenIdAuthorization> FindBySubjectAsync(string subject,
        CancellationToken cancellationToken);
    
    public abstract IAsyncEnumerable<OpenIdAuthorization> ListAsync(int? count, int? offset,
        CancellationToken cancellationToken);
    
    public abstract ValueTask PruneAsync(DateTimeOffset threshold, CancellationToken cancellationToken);

    public abstract ValueTask UpdateAsync(OpenIdAuthorization authorization, CancellationToken cancellationToken);
    
    
    
    public ValueTask<string?> GetApplicationIdAsync(OpenIdAuthorization authorization,
        CancellationToken cancellationToken)
        => new(authorization.ApplicationId);

    public ValueTask<DateTimeOffset?> GetCreationDateAsync(OpenIdAuthorization authorization,
        CancellationToken cancellationToken)
        => new(authorization.CreatedAt);

    public ValueTask<string?> GetIdAsync(OpenIdAuthorization authorization, CancellationToken cancellationToken)
        => new(authorization.Id.Value);

    public ValueTask<ImmutableDictionary<string, JsonElement>> GetPropertiesAsync(
        OpenIdAuthorization authorization, CancellationToken cancellationToken)
        => new(ImmutableDictionary<string, JsonElement>.Empty);

    public ValueTask<ImmutableArray<string>> GetScopesAsync(OpenIdAuthorization authorization,
        CancellationToken cancellationToken)
        => new(authorization.Scopes.ToImmutableArray());

    public ValueTask<string?> GetStatusAsync(OpenIdAuthorization authorization, CancellationToken cancellationToken)
        => new(authorization.Status);

    public ValueTask<string?> GetSubjectAsync(OpenIdAuthorization authorization, CancellationToken cancellationToken)
        => new(authorization.UserId);

    public ValueTask<string?> GetTypeAsync(OpenIdAuthorization authorization, CancellationToken cancellationToken)
        => new(authorization.Type);

    
    public virtual ValueTask<OpenIdAuthorization> InstantiateAsync(CancellationToken cancellationToken)
        => new(new OpenIdAuthorization());
    
    public ValueTask SetApplicationIdAsync(OpenIdAuthorization authorization, string? identifier,
        CancellationToken cancellationToken)
    {
        authorization.ApplicationId = identifier;

        return new ValueTask();
    }

    public ValueTask SetCreationDateAsync(OpenIdAuthorization authorization, DateTimeOffset? date,
        CancellationToken cancellationToken)
    {
        authorization.CreatedAt = date;

        return new ValueTask();
    }

    public ValueTask SetPropertiesAsync(OpenIdAuthorization authorization,
        ImmutableDictionary<string, JsonElement> properties,
        CancellationToken cancellationToken)
        => new();

    public ValueTask SetScopesAsync(OpenIdAuthorization authorization, ImmutableArray<string> scopes, CancellationToken cancellationToken)
    {
        authorization.Scopes = scopes.ToList();

        return new ValueTask();
    }

    public ValueTask SetStatusAsync(OpenIdAuthorization authorization, string? status, CancellationToken cancellationToken)
    {
        authorization.Status = status;

        return new ValueTask();
    }

    public ValueTask SetSubjectAsync(OpenIdAuthorization authorization, string? subject, CancellationToken cancellationToken)
    {
        authorization.UserId = subject;

        return new ValueTask();
    }

    public ValueTask SetTypeAsync(OpenIdAuthorization authorization, string? type, CancellationToken cancellationToken)
    {
        authorization.Type = type;

        return new ValueTask();
    }

    public ValueTask<long> CountAsync<TResult>(Func<IQueryable<OpenIdAuthorization>, IQueryable<TResult>> query,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();

    public ValueTask<TResult?> GetAsync<TState, TResult>(
        Func<IQueryable<OpenIdAuthorization>, TState, IQueryable<TResult>> query, TState state,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();

    public IAsyncEnumerable<TResult> ListAsync<TState, TResult>(
        Func<IQueryable<OpenIdAuthorization>, TState, IQueryable<TResult>> query, TState state,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();
}