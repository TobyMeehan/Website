using System.Runtime.CompilerServices;
using TobyMeehan.Com.Accounts.Models;
using TobyMeehan.Com.Accounts.Models.OpenId;
using TobyMeehan.Com.Builders;
using TobyMeehan.Com.Builders.Scope;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Accounts.OpenId;

public class OpenIdScopeStore : BaseScopeStore
{
    private readonly IScopeService _service;

    public OpenIdScopeStore(IScopeService service)
    {
        _service = service;
    }
    
    public override async ValueTask<long> CountAsync(CancellationToken cancellationToken)
    {
        return await _service.CountAsync(cancellationToken);
    }

    public override async ValueTask CreateAsync(OpenIdScope scope, CancellationToken cancellationToken)
    {
        if (scope is {Name: null} or {DisplayName: null} or {Description: null})
        {
            throw new ArgumentNullException(nameof(scope));
        }
        
        await _service.CreateAsync(new CreateScopeBuilder()
            .WithName(scope.Name)
            .WithDescription(scope.Description)
            .WithDisplayName(scope.DisplayName), cancellationToken);
    }

    public override async ValueTask DeleteAsync(OpenIdScope scope, CancellationToken cancellationToken)
    {
        await _service.DeleteAsync(scope.Id, cancellationToken);
    }

    private static OpenIdScope Map(IScope scope)
    {
        return new OpenIdScope
        {
            Id = scope.Id,
            Name = scope.Name,
            DisplayName = scope.DisplayName,
            Description = scope.Description
        };
    }
    
    public override async ValueTask<OpenIdScope?> FindByIdAsync(string identifier, CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(new Id<IScope>(identifier), cancellationToken: cancellationToken);

        return result.Match<OpenIdScope?>(
            scope => Map(scope),
            notFound => null);
    }

    public override async ValueTask<OpenIdScope?> FindByNameAsync(string name, CancellationToken cancellationToken)
    {
        var result = await _service.GetByNameAsync(name, cancellationToken: cancellationToken);

        return result.Match<OpenIdScope?>(
            scope => Map(scope),
            notFound => null);
    }

    public override async IAsyncEnumerable<OpenIdScope> ListAsync(int? count, int? offset, 
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var options = new QueryOptions
        {
            LimitStrategy = new OffsetLimitStrategy(count ?? 200, offset ?? 0)
        };

        await foreach (var scope in _service.GetAllAsync(options, cancellationToken))
        {
            yield return Map(scope);
        }
    }

    public override async ValueTask UpdateAsync(OpenIdScope scope, CancellationToken cancellationToken)
    {
        var update = new UpdateScopeBuilder();

        if (scope.DisplayName is { } displayName)
        {
            update.DisplayName = displayName;
        }

        if (scope.Description is { } description)
        {
            update.Description = description;
        }

        await _service.UpdateAsync(scope.Id, update, cancellationToken);
    }
}