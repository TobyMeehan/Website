using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using TobyMeehan.Com.Accounts.Models;
using TobyMeehan.Com.Accounts.Models.OpenId;
using TobyMeehan.Com.Builders;
using TobyMeehan.Com.Builders.Authorization;
using TobyMeehan.Com.Data;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Accounts.OpenId;

public class OpenIdAuthorizationStore : BaseAuthorizationStore
{
    private readonly IAuthorizationService _service;
    private readonly IApplicationService _applications;
    private readonly IUserService _users;
    private readonly IIdService _id;

    public OpenIdAuthorizationStore(
        IAuthorizationService service, 
        IApplicationService applications, 
        IUserService users,
        IIdService id)
    {
        _service = service;
        _applications = applications;
        _users = users;
        _id = id;
    }
    
    public override async ValueTask<long> CountAsync(CancellationToken cancellationToken)
    {
        return await _service.CountAsync(cancellationToken);
    }

    public override async ValueTask<OpenIdAuthorization> InstantiateAsync(CancellationToken cancellationToken)
    {
        var id = await _id.GenerateAsync<IAuthorization>();

        return new OpenIdAuthorization { Id = id };
    }

    public override async ValueTask CreateAsync(OpenIdAuthorization authorization, CancellationToken cancellationToken)
    {
        if (authorization is { ApplicationId: null } or { UserId: null })
        {
            return;
        }
        
        var applicationResult =
            await _applications.GetByIdAsync(new Id<IApplication>(authorization.ApplicationId), cancellationToken: cancellationToken);

        if (!applicationResult.IsSuccess(out var application))
        {
            return;
        }

        var userResult =
            await _users.GetByIdAsync(new Id<IUser>(authorization.UserId), cancellationToken: cancellationToken);

        if (!userResult.IsSuccess(out var user))
        {
            return;
        }
        
        var builder = new CreateAuthorizationBuilder()
            .WithApplication(application.Id)
            .WithUser(user.Id)
            .WithStatus(authorization.Status)
            .WithType(authorization.Type)
            .WithScopes(authorization.Scopes)
            .WithCreatedAt(authorization.CreatedAt?.UtcDateTime ?? DateTime.UtcNow);

        if (authorization.Id != default)
        {
            builder.Id = authorization.Id;
        }

        await _service.CreateAsync(builder, cancellationToken);
    }

    public override async ValueTask DeleteAsync(OpenIdAuthorization authorization, CancellationToken cancellationToken)
    {
        await _service.DeleteAsync(authorization.Id, cancellationToken);
    }

    private static OpenIdAuthorization Map(IAuthorization authorization)
    {
        return new OpenIdAuthorization
        {
            Id = authorization.Id,
            ApplicationId = authorization.ApplicationId.Value,
            UserId = authorization.UserId.Value,
            Status = authorization.Status,
            Type = authorization.Type,
            Scopes = authorization.Scopes.Select(x => x.Name).ToList(),
            CreatedAt = DateTime.SpecifyKind(authorization.CreatedAt, DateTimeKind.Utc)
        };
    }
    
    public override async IAsyncEnumerable<OpenIdAuthorization> FindAsync(string subject, string client, 
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var authorization in _service.GetByApplicationAndUserAsync(new Id<IApplication>(client), new Id<IUser>(subject),
                           cancellationToken: cancellationToken))
        {
            yield return Map(authorization);
        }
    }

    public override async IAsyncEnumerable<OpenIdAuthorization> FindAsync(string subject, string client, string status, 
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var authorization in FindAsync(subject, client, cancellationToken))
        {
            if (authorization.Status == status)
            {
                yield return authorization;
            }
        }
    }

    public override async IAsyncEnumerable<OpenIdAuthorization> FindAsync(string subject, string client, string status, string type,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var authorization in FindAsync(subject, client, cancellationToken))
        {
            if (authorization.Type == type)
            {
                yield return authorization;
            }
        }
    }

    public override async IAsyncEnumerable<OpenIdAuthorization> FindAsync(string subject, string client, string status, string type, ImmutableArray<string> scopes,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var authorization in FindAsync(subject, client, status, type, cancellationToken))
        {
            if (authorization.Scopes.Any(scopes.Contains))
            {
                yield return authorization;
            }
        }
    }

    public override async IAsyncEnumerable<OpenIdAuthorization> FindByApplicationIdAsync(string identifier, 
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var authorization in _service.GetByApplicationAsync(new Id<IApplication>(identifier),
                           cancellationToken: cancellationToken))
        {
            yield return Map(authorization);
        }
    }

    public override async ValueTask<OpenIdAuthorization?> FindByIdAsync(string identifier, CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(new Id<IAuthorization>(identifier), cancellationToken: cancellationToken);

        return result.Match<OpenIdAuthorization?>(
            authorization => Map(authorization),
            notFound => null);
    }

    public override async IAsyncEnumerable<OpenIdAuthorization> FindBySubjectAsync(string subject, 
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var authorization in _service.GetByUserAsync(new Id<IUser>(subject), cancellationToken: cancellationToken))
        {
            yield return Map(authorization);
        }
    }

    public override async IAsyncEnumerable<OpenIdAuthorization> ListAsync(int? count, int? offset, 
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var options = new QueryOptions
        {
            LimitStrategy = new OffsetLimitStrategy(count ?? 200, offset ?? 0)
        };

        await foreach (var authorization in _service.GetAllAsync(options, cancellationToken))
        {
            yield return Map(authorization);
        }
    }

    public override async ValueTask PruneAsync(DateTimeOffset threshold, CancellationToken cancellationToken)
    {
        await _service.DeleteByCreationAsync(threshold.UtcDateTime, cancellationToken);
    }

    public override async ValueTask UpdateAsync(OpenIdAuthorization authorization, CancellationToken cancellationToken)
    {
        await _service.UpdateAsync(authorization.Id, new UpdateAuthorizationBuilder()
            .WithStatus(authorization.Status)
            .WithType(authorization.Type)
            .WithScopes(authorization.Scopes)
            .WithCreatedAt(authorization.CreatedAt?.UtcDateTime ?? DateTime.UtcNow), cancellationToken);
    }
}