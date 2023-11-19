using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using OpenIddict.Abstractions;
using TobyMeehan.Com.Accounts.Models;
using TobyMeehan.Com.Accounts.Models.OpenId;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Accounts.OpenId;

public class OpenIdApplicationStore : BaseApplicationStore
{
    private readonly IApplicationService _service;

    public OpenIdApplicationStore(IApplicationService service)
    {
        _service = service;
    }
    
    public override async ValueTask<long> CountAsync(CancellationToken cancellationToken)
    {
        return await _service.CountAsync(cancellationToken);
    }

    public override async ValueTask<OpenIdApplication?> FindByIdAsync(string identifier, CancellationToken cancellationToken)
    {
        var result =
            await _service.GetByIdAsync(new Id<IApplication>(identifier), cancellationToken: cancellationToken);

        return result.Match<OpenIdApplication?>(
            application => new OpenIdApplication
            {
                Id = application.Id,
                Name = application.Name,
                ClientType = application.HasSecret ? OpenIddictConstants.ClientTypes.Confidential : OpenIddictConstants.ClientTypes.Public,
                Redirects = application.Redirects
            },
            notFound => null);
    }

    public override async ValueTask<OpenIdApplication?> FindByClientIdAsync(string identifier,
        CancellationToken cancellationToken)
        => await FindByIdAsync(identifier, cancellationToken);

    public override async IAsyncEnumerable<OpenIdApplication> FindByRedirectUriAsync(string uri, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var application in _service.GetByRedirectAsync(uri, cancellationToken: cancellationToken))
        {
            yield return new OpenIdApplication
            {
                Id = application.Id,
                Name = application.Name,
                ClientType = application.HasSecret ? OpenIddictConstants.ClientTypes.Confidential : OpenIddictConstants.ClientTypes.Public,
                Redirects = application.Redirects
            };
        }
    }
    
    public override async IAsyncEnumerable<OpenIdApplication> ListAsync(int? count, int? offset, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var options = new QueryOptions
        {
            LimitStrategy = new OffsetLimitStrategy(count ?? 200, offset ?? 0)
        };
        
        await foreach (var application in _service.GetAllAsync(options, cancellationToken))
        {
            yield return new OpenIdApplication
            {
                Id = application.Id,
                Name = application.Name,
                ClientType = application.HasSecret ? OpenIddictConstants.ClientTypes.Confidential : OpenIddictConstants.ClientTypes.Public,
                Redirects = application.Redirects
            };
        }
    }
    
    public override ValueTask<ImmutableArray<string>> GetRedirectUrisAsync(OpenIdApplication application, CancellationToken cancellationToken)
    {
        return new ValueTask<ImmutableArray<string>>(
            application.Redirects.Select(x => x.Uri.OriginalString).ToImmutableArray());
    }

    public override ValueTask<ImmutableArray<string>> GetPermissionsAsync(OpenIdApplication application, CancellationToken cancellationToken)
    {
        var permissions = new List<string>
        {
            OpenIddictConstants.Permissions.Endpoints.Authorization,
            OpenIddictConstants.Permissions.Endpoints.Token,
            OpenIddictConstants.Permissions.Endpoints.Logout,
            OpenIddictConstants.Permissions.Endpoints.Revocation,
            
            OpenIddictConstants.Permissions.ResponseTypes.Code,
            OpenIddictConstants.Permissions.ResponseTypes.Token,
            
            OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
            OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,
            OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
            OpenIddictConstants.Permissions.GrantTypes.Implicit
        };
        
        permissions.AddRange(
            Scope.All.Select(x => OpenIddictConstants.Permissions.Prefixes.Scope + x));

        return new ValueTask<ImmutableArray<string>>(permissions.ToImmutableArray());
    }

    public override ValueTask<ImmutableArray<string>> GetRequirementsAsync(OpenIdApplication application, CancellationToken cancellationToken)
    {
        return new ValueTask<ImmutableArray<string>>(ImmutableArray.Create<string>());
    }
}