using System.Diagnostics;
using System.Runtime.CompilerServices;
using TobyMeehan.Com.Accounts.Models;
using TobyMeehan.Com.Accounts.Models.OpenId;
using TobyMeehan.Com.Builders;
using TobyMeehan.Com.Builders.Token;
using TobyMeehan.Com.Data;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Accounts.OpenId;

public class OpenIdTokenStore : BaseTokenStore
{
    private readonly ITokenService _service;
    private readonly IAuthorizationService _authorizations;
    private readonly IIdService _id;

    public OpenIdTokenStore(
        ITokenService service, 
        IAuthorizationService authorizations,
        IIdService id)
    {
        _service = service;
        _authorizations = authorizations;
        _id = id;
    }
    
    public override async ValueTask<long> CountAsync(CancellationToken cancellationToken)
    {
        return await _service.CountAsync(cancellationToken);
    }

    public override async ValueTask<OpenIdToken> InstantiateAsync(CancellationToken cancellationToken)
    {
        var id = await _id.GenerateAsync<IToken>();

        return new OpenIdToken { Id = id };
    }

    public override async ValueTask CreateAsync(OpenIdToken token, CancellationToken cancellationToken)
    {
        if (token.AuthorizationId is null)
        {
            return;
        }

        var authorizationResult = await _authorizations.GetByIdAsync(new Id<IAuthorization>(token.AuthorizationId),
            cancellationToken: cancellationToken);

        if (!authorizationResult.IsSuccess(out var authorization))
        {
            return;
        }

        var builder = new CreateTokenBuilder()
            .WithAuthorization(authorization.Id)
            .WithPayload(token.Payload)
            .WithReferenceId(token.ReferenceId)
            .WithStatus(token.Status)
            .WithType(token.Type)
            .WithSubject(token.Subject)
            .WithRedeemedAt(token.RedeemedAt?.UtcDateTime)
            .WithExpiresAt(token.ExpiresAt?.UtcDateTime)
            .WithCreatedAt(token.CreatedAt?.UtcDateTime ?? DateTime.UtcNow);

        if (token.Id != default)
        {
            builder.Id = token.Id;
        }
        
        await _service.CreateAsync(builder, cancellationToken);
    }

    public override async ValueTask DeleteAsync(OpenIdToken token, CancellationToken cancellationToken)
    {
        await _service.DeleteAsync(token.Id, cancellationToken);
    }

    private static OpenIdToken Map(IToken token)
    {
        return new OpenIdToken
        {
            Id = token.Id,
            AuthorizationId = token.Authorization?.Id.Value,
            ApplicationId = token.Authorization?.ApplicationId.Value,
            Subject = token.Subject,
            Payload = token.Payload,
            ReferenceId = token.ReferenceId,
            Status = token.Status,
            Type = token.Type,
            RedeemedAt = token.RedemptionDate.HasValue 
                ? DateTime.SpecifyKind(token.RedemptionDate.Value, DateTimeKind.Utc) : null,
            ExpiresAt = token.ExpiresAt.HasValue 
                ? DateTime.SpecifyKind(token.ExpiresAt.Value, DateTimeKind.Utc) : null,
            CreatedAt = DateTime.SpecifyKind(token.CreatedAt, DateTimeKind.Utc)
        };
    }
    
    public override async IAsyncEnumerable<OpenIdToken> FindAsync(string subject, string client, 
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var token in _service.GetByApplicationAndSubjectAsync(new Id<IApplication>(client), subject, cancellationToken: cancellationToken))
        {
            yield return Map(token);
        }
        
    }

    public override async IAsyncEnumerable<OpenIdToken> FindAsync(string subject, string client, string status, 
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var token in _service.GetByApplicationAndSubjectAsync(new Id<IApplication>(client), subject,
                           cancellationToken: cancellationToken))
        {
            if (token.Status != status)
            {
                continue;
            }
            
            yield return Map(token);
        }
    }

    public override async IAsyncEnumerable<OpenIdToken> FindAsync(string subject, string client, string status, string type,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var token in _service.GetByApplicationAndSubjectAsync(new Id<IApplication>(client), subject, cancellationToken: cancellationToken))
        {
            if (token.Status != status || token.Type != type)
            {
                continue;
            }

            yield return Map(token);
        }
    }

    public override async IAsyncEnumerable<OpenIdToken> FindByApplicationIdAsync(string identifier, 
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var token in _service.GetByApplicationAsync(new Id<IApplication>(identifier), cancellationToken: cancellationToken))
        {
            yield return Map(token);
        }
    }

    public override async IAsyncEnumerable<OpenIdToken> FindByAuthorizationIdAsync(string identifier, 
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var token in _service.GetByAuthorizationAsync(new Id<IAuthorization>(identifier), cancellationToken: cancellationToken))
        {
            yield return Map(token);
        }
    }

    public override async ValueTask<OpenIdToken?> FindByIdAsync(string identifier, CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(new Id<IToken>(identifier), cancellationToken: cancellationToken);

        return result.Match<OpenIdToken?>(
            token => Map(token),
            notFound => null);
    }

    public override async ValueTask<OpenIdToken?> FindByReferenceIdAsync(string identifier, CancellationToken cancellationToken)
    {
        var result = await _service.GetByReferenceIdAsync(identifier, cancellationToken: cancellationToken);

        return result.Match<OpenIdToken?>(
            token => Map(token),
            notFound => null);
    }

    public override async IAsyncEnumerable<OpenIdToken> FindBySubjectAsync(string subject, 
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var token in _service.GetBySubjectAsync(new Id<IUser>(subject), cancellationToken: cancellationToken))
        {
            yield return Map(token);
        }
    }

    public override async IAsyncEnumerable<OpenIdToken> ListAsync(int? count, int? offset, 
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var token in _service.GetAllAsync(
                           new QueryOptions { LimitStrategy = new OffsetLimitStrategy(count ?? 200, offset ?? 0) },
                           cancellationToken))
        {
            yield return Map(token);
        }
    }

    public override async ValueTask PruneAsync(DateTimeOffset threshold, CancellationToken cancellationToken)
    {
        await _service.DeleteByExpirationAsync(threshold.UtcDateTime, cancellationToken);
    }

    public override async ValueTask UpdateAsync(OpenIdToken token, CancellationToken cancellationToken)
    {
        await _service.UpdateAsync(token.Id, new UpdateTokenBuilder()
            .WithReferenceId(token.ReferenceId)
            .WithPayload(token.Payload)
            .WithStatus(token.Status)
            .WithRedeemedAt(token.RedeemedAt?.UtcDateTime)
            .WithExpiresAt(token.ExpiresAt?.UtcDateTime), cancellationToken);
    }
}