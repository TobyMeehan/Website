using OneOf;
using TobyMeehan.Com.Data.Caching;
using TobyMeehan.Com.Data.Domain.Authorizations.Models;
using TobyMeehan.Com.Data.Domain.Authorizations.Repositories;
using TobyMeehan.Com.Models.Authorization;
using TobyMeehan.Com.Results;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Data.Domain.Authorizations;

public class AuthorizationService : BaseService<IAuthorization, AuthorizationDto>, IAuthorizationService
{
    private readonly IAuthorizationRepository _db;
    private readonly IScopeService _scopes;
    private readonly IIdService _id;

    public AuthorizationService(
        IAuthorizationRepository db, 
        IScopeService scopes, 
        IIdService id, 
        ICacheService<AuthorizationDto, Id<IAuthorization>> cache) : base(cache)
    {
        _db = db;
        _scopes = scopes;
        _id = id;
    }

    protected override async Task<IAuthorization> MapAsync(AuthorizationDto data)
    {
        var scopes = new EntityCollection<IScope>();
        
        foreach (string scopeId in data.Scopes.Split())
        {
            var result = await _scopes.GetByIdAsync(new Id<IScope>(scopeId));

            result.Switch(
                scope => scopes.Add(scope),
                _ => {});
        }

        return new Models.Authorization
        {
            Id = new Id<IAuthorization>(data.Id),
            ApplicationId = new Id<IApplication>(data.ApplicationId),
            UserId = new Id<IUser>(data.UserId),
            Status = data.Status,
            Type = data.Type,
            Scopes = scopes,
            CreatedAt = data.CreatedAt
        };
    }

    public IAsyncEnumerable<IAuthorization> GetByApplicationAsync(Id<IApplication> application,
        QueryOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var data = _db.SelectByApplicationAsync(application.Value, options?.LimitStrategy, cancellationToken);

        return GetAsync(data);
    }

    public IAsyncEnumerable<IAuthorization> GetByUserAsync(Id<IUser> user, QueryOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var data = _db.SelectByUserAsync(user.Value, options?.LimitStrategy, cancellationToken);

        return GetAsync(data);
    }

    public IAsyncEnumerable<IAuthorization> GetByApplicationAndUserAsync(Id<IApplication> application, Id<IUser> user,
        QueryOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var data = _db.SelectByApplicationAndUserAsync(application.Value, user.Value, options?.LimitStrategy,
            cancellationToken);

        return GetAsync(data);
    }

    public async Task<OneOf<IAuthorization, NotFound>> GetByIdAsync(Id<IAuthorization> id,
        QueryOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var data = Cache.Get(id) ?? await _db.SelectByIdAsync(id.Value, cancellationToken);

        if (data is null)
        {
            return new NotFound();
        }
        
        return OneOf<IAuthorization, NotFound>.FromT0(
            await GetAsync(data));
    }

    public IAsyncEnumerable<IAuthorization> GetAllAsync(QueryOptions? options = null, CancellationToken cancellationToken = default)
    {
        var data = _db.SelectAllAsync(options?.LimitStrategy, cancellationToken);

        return GetAsync(data);
    }

    public async Task<long> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _db.CountAsync(cancellationToken);
    }

    public async Task<IAuthorization> CreateAsync(ICreateAuthorization create, CancellationToken cancellationToken = default)
    {
        var id = create.Id | await _id.GenerateAsync<IAuthorization>();

        var data = new AuthorizationDto
        {
            Id = id.Value,
            ApplicationId = create.Application.Value,
            UserId = create.User.Value,

            Status = create.Status,
            Type = create.Type,

            Scopes = string.Join(' ', create.Scopes),

            CreatedAt = create.CreatedAt
        };

        await _db.InsertAsync(data, cancellationToken);
        
        Cache.Set(id, data);

        return await MapAsync(data);
    }

    public async Task<OneOf<IAuthorization, NotFound>> UpdateAsync(Id<IAuthorization> id, IUpdateAuthorization update,
        CancellationToken cancellationToken = default)
    {
        var data = await _db.SelectByIdAsync(id.Value, cancellationToken);

        if (data is null)
        {
            return new NotFound();
        }

        data.Type = update.Type | data.Type;
        data.Status = update.Status | data.Status;
        data.Scopes = update.Scopes.MapOr(map => string.Join(' ', map), data.Scopes);

        await _db.UpdateAsync(id.Value, data, cancellationToken);

        return OneOf<IAuthorization, NotFound>.FromT0(
            await GetAsync(data));
    }

    public async Task<OneOf<Success, NotFound>> DeleteAsync(Id<IAuthorization> id,
        CancellationToken cancellationToken = default)
    {
        int result = await _db.DeleteAsync(id.Value, cancellationToken);
        
        Cache.Remove(id);

        return result == 1 ? new Success() : new NotFound();
    }

    public async Task DeleteByCreationAsync(DateTime threshold, CancellationToken cancellationToken = default)
    {
        await _db.DeleteByCreationAsync(threshold, cancellationToken);
        
        Cache.RemoveWhere(x => x.CreatedAt > threshold);
    }
}