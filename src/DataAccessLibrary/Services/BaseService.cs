using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Caching.Memory;
using TobyMeehan.Com.Data.Entities;
using TobyMeehan.Com.Data.Repositories;
using TobyMeehan.Com.Data.Repositories.Models;
using TobyMeehan.Com.Exceptions;

namespace TobyMeehan.Com.Data.Services;

public abstract class BaseService<TEntity, TData> where TEntity : IEntity<TEntity>
{
    private readonly IRepositorySelect<TData> _select;
    private readonly IRepositoryUpdate<TData> _update;
    private readonly IRepositoryDelete _delete;

    public BaseService(IRepository<TData> repository, IMemoryCache cache)
    {
        Cache = cache;
        _select = repository;
        _update = repository;
        _delete = repository;
    }
    
    public BaseService(IRepositorySelect<TData> select, IRepositoryUpdate<TData> update, IRepositoryDelete delete, IMemoryCache cache)
    {
        _select = select;
        _update = update;
        _delete = delete;
        Cache = cache;
    }
    
    protected IMemoryCache Cache { get; }
    
    protected async Task<IEntityCollection<TEntity>> GetAsync(IEnumerable<TData> data)
    {
        EntityCollection<TEntity> collection = new();

        foreach (var item in data)
        {
            collection.Add(await MapAsync(item));
        }

        return collection;
    }

    protected async Task<TEntity> GetAsync(TData data)
    {
        var entity = await MapAsync(data);

        Cache.Set(entity.Id, data);

        return entity;
    }
    
    protected abstract Task<TEntity> MapAsync(TData data);

    protected async Task<TData?> TryGetCacheAsync(Id<TEntity> id, CancellationToken cancellationToken = default)
    {
        if (Cache.TryGetValue<TData>(id, out var data))
        {
            return data;
        }

        return await _select.SelectByIdAsync(id.Value, cancellationToken);
    }

    public async Task<TEntity?> FindByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var data = await _select.SelectByIdAsync(id, cancellationToken);

        if (data is null)
        {
            return default;
        }
        
        return await GetAsync(data);
    }
    
    public async Task<TEntity> GetByIdAsync(Id<TEntity> id, CancellationToken cancellationToken = default)
    {
        var data = await TryGetCacheAsync(id, cancellationToken);

        if (data is null)
        {
            throw new EntityNotFoundException<TEntity>(id);
        }
        
        return await GetAsync(data);
    }
    
    public async Task<IEntityCollection<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var data = await _select.SelectAllAsync(cancellationToken);

        return await GetAsync(data);
    }
    
    protected async Task<TEntity> UpdateAsync(Id<TEntity> id, Action<TData> patch, CancellationToken cancellationToken)
    {
        var data = await TryGetCacheAsync(id, cancellationToken);

        if (data is null)
        {
            throw new EntityNotFoundException<TEntity>(id);
        }
        
        patch(data);

        await _update.UpdateAsync(id.Value, data, cancellationToken);

        return await GetAsync(data);
    }

    public async Task DeleteAsync(Id<TEntity> id, CancellationToken cancellationToken = default)
    {
        Cache.Remove(id);
        
        await _delete.DeleteAsync(id.Value, cancellationToken);
    }
}

public abstract class BaseService<TEntity, TData, TCreate> : BaseService<TEntity, TData> where TEntity : IEntity<TEntity>
{
    
    private readonly IRepositoryInsert<TData> _insert;

    public BaseService(IRepository<TData> repository, IMemoryCache cache) : base(repository, cache)
    {
        _insert = repository;
    }

    public BaseService(IRepositorySelect<TData> select, IRepositoryInsert<TData> insert, IRepositoryUpdate<TData> update, IRepositoryDelete delete, IMemoryCache cache) 
        : base(select, update, delete, cache)
    {
        _insert = insert;
    }    
    
    protected abstract Task<TData> CreateAsync(TCreate create);
    
    public async Task<TEntity> CreateAsync(TCreate create, CancellationToken cancellationToken)
    {
        var data = await CreateAsync(create);

        await _insert.InsertAsync(data, cancellationToken);

        return await GetAsync(data);
    }
}