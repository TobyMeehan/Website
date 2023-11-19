using System.Collections.Immutable;
using System.Text.Json;
using OpenIddict.Abstractions;
using TobyMeehan.Com.Accounts.Models;
using TobyMeehan.Com.Accounts.Models.OpenId;

namespace TobyMeehan.Com.Accounts.OpenId;

public abstract class BaseTokenStore : IOpenIddictTokenStore<OpenIdToken>
{
    public abstract ValueTask<long> CountAsync(CancellationToken cancellationToken);

    public abstract ValueTask CreateAsync(OpenIdToken token, CancellationToken cancellationToken);

    public abstract ValueTask DeleteAsync(OpenIdToken token, CancellationToken cancellationToken);

    public abstract IAsyncEnumerable<OpenIdToken> FindAsync(string subject, string client,
        CancellationToken cancellationToken);

    public abstract IAsyncEnumerable<OpenIdToken> FindAsync(string subject, string client, string status,
        CancellationToken cancellationToken);

    public abstract IAsyncEnumerable<OpenIdToken> FindAsync(string subject, string client, string status, string type,
        CancellationToken cancellationToken);

    public abstract IAsyncEnumerable<OpenIdToken> FindByApplicationIdAsync(string identifier,
        CancellationToken cancellationToken);

    public abstract IAsyncEnumerable<OpenIdToken> FindByAuthorizationIdAsync(string identifier,
        CancellationToken cancellationToken);

    public abstract ValueTask<OpenIdToken?> FindByIdAsync(string identifier, CancellationToken cancellationToken);

    public abstract ValueTask<OpenIdToken?> FindByReferenceIdAsync(string identifier,
        CancellationToken cancellationToken);

    public abstract IAsyncEnumerable<OpenIdToken> FindBySubjectAsync(string subject,
        CancellationToken cancellationToken);
    
    public abstract IAsyncEnumerable<OpenIdToken> ListAsync(int? count, int? offset,
        CancellationToken cancellationToken);

    public abstract ValueTask PruneAsync(DateTimeOffset threshold, CancellationToken cancellationToken);

    public abstract ValueTask UpdateAsync(OpenIdToken token, CancellationToken cancellationToken);

    
    public ValueTask<string?> GetApplicationIdAsync(OpenIdToken token, CancellationToken cancellationToken)
        => new(token.ApplicationId);

    public ValueTask<string?> GetAuthorizationIdAsync(OpenIdToken token, CancellationToken cancellationToken)
        => new(token.AuthorizationId);

    public ValueTask<DateTimeOffset?> GetCreationDateAsync(OpenIdToken token, CancellationToken cancellationToken)
        => new(token.CreatedAt);

    public ValueTask<DateTimeOffset?> GetExpirationDateAsync(OpenIdToken token, CancellationToken cancellationToken)
        => new(token.ExpiresAt);

    public ValueTask<string?> GetIdAsync(OpenIdToken token, CancellationToken cancellationToken)
        => new(token.Id.Value);

    public ValueTask<string?> GetPayloadAsync(OpenIdToken token, CancellationToken cancellationToken)
        => new(token.Payload);

    public ValueTask<ImmutableDictionary<string, JsonElement>> GetPropertiesAsync(OpenIdToken token,
        CancellationToken cancellationToken)
        => new(ImmutableDictionary<string, JsonElement>.Empty);

    public ValueTask<DateTimeOffset?> GetRedemptionDateAsync(OpenIdToken token, CancellationToken cancellationToken)
        => new(token.RedeemedAt);

    public ValueTask<string?> GetReferenceIdAsync(OpenIdToken token, CancellationToken cancellationToken)
        => new(token.ReferenceId);

    public ValueTask<string?> GetStatusAsync(OpenIdToken token, CancellationToken cancellationToken)
        => new(token.Status);

    public ValueTask<string?> GetSubjectAsync(OpenIdToken token, CancellationToken cancellationToken)
        => new(token.Subject);

    public ValueTask<string?> GetTypeAsync(OpenIdToken token, CancellationToken cancellationToken)
        => new(token.Type);

    public virtual ValueTask<OpenIdToken> InstantiateAsync(CancellationToken cancellationToken)
    {
        return new ValueTask<OpenIdToken>(new OpenIdToken());
    }
    

    public ValueTask SetApplicationIdAsync(OpenIdToken token, string? identifier, CancellationToken cancellationToken)
    {
        token.ApplicationId = identifier;

        return new ValueTask();
    }

    public ValueTask SetAuthorizationIdAsync(OpenIdToken token, string? identifier, CancellationToken cancellationToken)
    {
        token.AuthorizationId = identifier;

        return new ValueTask();
    }

    public ValueTask SetCreationDateAsync(OpenIdToken token, DateTimeOffset? date, CancellationToken cancellationToken)
    {
        token.CreatedAt = date;

        return new ValueTask();
    }

    public ValueTask SetExpirationDateAsync(OpenIdToken token, DateTimeOffset? date, CancellationToken cancellationToken)
    {
        token.ExpiresAt = date;

        return new ValueTask();
    }

    public ValueTask SetPayloadAsync(OpenIdToken token, string? payload, CancellationToken cancellationToken)
    {
        token.Payload = payload;

        return new ValueTask();
    }

    public ValueTask SetPropertiesAsync(OpenIdToken token, ImmutableDictionary<string, JsonElement> properties,
        CancellationToken cancellationToken)
        => new();

    public ValueTask SetRedemptionDateAsync(OpenIdToken token, DateTimeOffset? date, CancellationToken cancellationToken)
    {
        token.RedeemedAt = date;

        return new ValueTask();
    }

    public ValueTask SetReferenceIdAsync(OpenIdToken token, string? identifier, CancellationToken cancellationToken)
    {
        token.ReferenceId = identifier;

        return new ValueTask();
    }

    public ValueTask SetStatusAsync(OpenIdToken token, string? status, CancellationToken cancellationToken)
    {
        token.Status = status;

        return new ValueTask();
    }

    public ValueTask SetSubjectAsync(OpenIdToken token, string? subject, CancellationToken cancellationToken)
    {
        token.Subject = subject;

        return new ValueTask();
    }

    public ValueTask SetTypeAsync(OpenIdToken token, string? type, CancellationToken cancellationToken)
    {
        token.Type = type;

        return new ValueTask();
    }
    
    public ValueTask<long> CountAsync<TResult>(Func<IQueryable<OpenIdToken>, IQueryable<TResult>> query,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();
    
    public ValueTask<TResult?> GetAsync<TState, TResult>(
        Func<IQueryable<OpenIdToken>, TState, IQueryable<TResult>> query, TState state,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();
    
    public IAsyncEnumerable<TResult> ListAsync<TState, TResult>(
        Func<IQueryable<OpenIdToken>, TState, IQueryable<TResult>> query, TState state,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();
}