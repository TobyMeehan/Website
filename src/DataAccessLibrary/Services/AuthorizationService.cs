using Microsoft.Extensions.Caching.Memory;
using TobyMeehan.Com.Builders;
using TobyMeehan.Com.Builders.Authorization;
using TobyMeehan.Com.Data.Caching;
using TobyMeehan.Com.Data.Entities;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories;
using TobyMeehan.Com.Exceptions;
using TobyMeehan.Com.Models.Authorization;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Data.Services;

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
        
        foreach (var scopeDto in data.Scopes)
        {
            if (await _scopes.FindByIdAsync(scopeDto.Id) is { } scope)
            {
                scopes.Add(scope);
            }
        }

        return new Authorization
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

    public IAsyncEnumerable<IAuthorization> FindByApplicationAsync(string applicationId, QueryOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var data = _db.SelectByApplicationAsync(applicationId, options?.LimitStrategy, cancellationToken);

        return GetAsync(data);
    }

    public IAsyncEnumerable<IAuthorization> FindByUserAsync(string userId, QueryOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var data = _db.SelectByUserAsync(userId, options?.LimitStrategy, cancellationToken);

        return GetAsync(data);
    }

    public IAsyncEnumerable<IAuthorization> FindByApplicationAndUserAsync(string applicationId, string userId, QueryOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var data = _db.SelectByApplicationAndUserAsync(applicationId, userId, options?.LimitStrategy,
            cancellationToken);

        return GetAsync(data);
    }

    public async Task<IAuthorization?> FindByIdAsync(string id, QueryOptions? options = null, CancellationToken cancellationToken = default)
    {
        var data = Cache.Get(x => x.Id == id) ?? await _db.SelectByIdAsync(id, cancellationToken);

        return await GetAsync(data);
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
        var id = await _id.GenerateAsync<IAuthorization>();

        var data = new AuthorizationDto
        {
            Id = id.Value,
            ApplicationId = create.Application.Value,
            UserId = create.User.Value,

            Status = create.Status,
            Type = create.Type,

            Scopes = create.Scopes.Select(x => new AuthorizationScopeDto
            {
                Id = x
            }).ToList(),

            CreatedAt = create.CreatedAt
        };

        await _db.InsertAsync(data, cancellationToken);
        
        Cache.Set(id, data);

        return await MapAsync(data);
    }

    public async Task<IAuthorization> UpdateAsync(Id<IAuthorization> id, IUpdateAuthorization update, CancellationToken cancellationToken = default)
    {
        var data = await _db.SelectByIdAsync(id.Value, cancellationToken);

        if (data is null)
        {
            throw new EntityNotFoundException<IAuthorization>(id);
        }

        data.Type = update.Type | data.Type;
        data.Status = update.Status | data.Status;
        data.Scopes = update.Scopes.MapOr(map => 
            map.Select(x => 
                new AuthorizationScopeDto
                {
                    Id = x
                })
                .ToList(), data.Scopes);

        await _db.UpdateAsync(id.Value, data, cancellationToken);
        
        Cache.Set(id, data);

        return await MapAsync(data);
    }

    public async Task DeleteAsync(Id<IAuthorization> id, CancellationToken cancellationToken = default)
    {
        await _db.DeleteAsync(id.Value, cancellationToken);
        
        Cache.Remove(id);
    }

    public async Task DeleteByCreationAsync(DateTime threshold, CancellationToken cancellationToken = default)
    {
        await _db.DeleteByCreationAsync(threshold, cancellationToken);
        
        Cache.RemoveWhere(x => x.CreatedAt > threshold);
    }
}