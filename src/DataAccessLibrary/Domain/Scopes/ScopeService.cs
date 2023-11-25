using OneOf;
using TobyMeehan.Com.Data.Authorization;
using TobyMeehan.Com.Data.Caching;
using TobyMeehan.Com.Data.Domain.Scopes.Models;
using TobyMeehan.Com.Data.Domain.Scopes.Repositories;
using TobyMeehan.Com.Data.Domain.UserRoles.Models;
using TobyMeehan.Com.Models.Scope;
using TobyMeehan.Com.Results;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Data.Domain.Scopes;

public class ScopeService : BaseService<IScope, ScopeDto>, IScopeService
{
    private readonly IScopeRepository _db;
    private readonly IIdService _id;
    private readonly IEnumerable<IScopeValidator> _validators;

    public ScopeService(
        IScopeRepository db, 
        IIdService id, 
        IEnumerable<IScopeValidator> validators,
        ICacheService<ScopeDto, Id<IScope>> cache) : base(cache)
    {
        _db = db;
        _id = id;
        _validators = validators;
    }

    private IUserRole MapUserRole(RoleDto role)
    {
        return new UserRole
        {
            Id = new Id<IUserRole>(role.Id),
            Name = role.Name
        };
    }
    
    protected override async Task<IScope> MapAsync(ScopeDto data)
    {
        return new Scope
        {
            Id = new Id<IScope>(data.Id),
            Alias = data.Alias,
            Name = data.Name,
            DisplayName = data.DisplayName,
            Description = data.Description,
            UserRoles = EntityCollection<IUserRole>.Create(data.UserRoles, MapUserRole)
        };
    }


    public IAsyncEnumerable<IScope> GetAllAsync(QueryOptions? options = null, CancellationToken cancellationToken = default)
    {
        var data = _db.SelectAllAsync(options?.LimitStrategy, cancellationToken);

        return GetAsync(data);
    }

    public async Task<OneOf<IScope, NotFound>> GetByIdAsync(Id<IScope> id, QueryOptions? options = null, CancellationToken cancellationToken = default)
    {
        var data = Cache.Get(id) ?? await _db.SelectByIdAsync(id.Value, cancellationToken);

        if (data is null)
        {
            return new NotFound();
        }

        return await GetAsync<Scope>(data);
    }

    public async Task<OneOf<IScope, NotFound>> GetByNameAsync(string name, QueryOptions? options = null, CancellationToken cancellationToken = default)
    {
        var data = Cache.Get(x => x.Name == name) ?? await _db.SelectByNameAsync(name, cancellationToken);

        if (data is null)
        {
            return new NotFound();
        }

        return await GetAsync<Scope>(data);
    }

    public async Task<OneOf<IScope, NotFound>> GetByAliasAsync(string alias, QueryOptions? options = null, CancellationToken cancellationToken = default)
    {
        var data = Cache.Get(x => x.Alias == alias || x.Name == alias)
                   ?? await _db.SelectByAliasAsync(alias, cancellationToken);

        if (data is null)
        {
            return new NotFound();
        }

        return await GetAsync<Scope>(data);
    }

    public async Task<long> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _db.CountAsync(cancellationToken);
    }

    public async Task<IScope> CreateAsync(ICreateScope create, CancellationToken cancellationToken = default)
    {
        var id = await _id.GenerateAsync<IScope>();

        var data = new ScopeDto
        {
            Id = id.Value,
            Alias = create.Alias | create.Name,
            Name = create.Name,
            DisplayName = create.DisplayName,
            Description = create.Description
        };

        await _db.InsertAsync(data, cancellationToken);

        return await GetAsync<Scope>(data);
    }

    public async Task<OneOf<IScope, NotFound>> UpdateAsync(Id<IScope> id, IUpdateScope update, CancellationToken cancellationToken = default)
    {
        var data = await _db.SelectByIdAsync(id.Value, cancellationToken);

        if (data is null)
        {
            return new NotFound();
        }

        data.DisplayName = update.DisplayName | data.DisplayName;
        data.Description = update.Description | data.Description;

        await _db.UpdateAsync(id.Value, data, cancellationToken);

        return await GetAsync<Scope>(data);
    }

    public async Task<OneOf<Success, NotFound>> DeleteAsync(Id<IScope> id, CancellationToken cancellationToken = default)
    {
        int result = await _db.DeleteAsync(id.Value, cancellationToken);
        
        Cache.Remove(id);

        return result == 1 ? new Success() : new NotFound();
    }

    public async Task<OneOf<Success, Forbidden>> AuthorizeScopeAsync(IScope scope, IUser user, IApplication application,
        CancellationToken cancellationToken = default)
    {
        foreach (var validator in _validators)
        {
            if (!await validator.CanValidateAsync(scope, cancellationToken))
            {
                continue;
            }

            if (await validator.ValidateAsync(scope, user, application, cancellationToken))
            {
                continue;
            }

            return new Forbidden();
        }

        return new Success();
    }
}