using System.Diagnostics.CodeAnalysis;
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
    
    public BaseService(IRepository<TData> repository)
    {
        _select = repository;
        _update = repository;
        _delete = repository;
    }
    
    public BaseService(IRepositorySelect<TData> select, IRepositoryUpdate<TData> update, IRepositoryDelete delete)
    {
        _select = select;
        _update = update;
        _delete = delete;
    }
    
    protected async Task<IEntityCollection<TEntity>> MapAsync(IEnumerable<TData> data)
    {
        EntityCollection<TEntity> collection = new();

        foreach (var item in data)
        {
            collection.Add(await MapperAsync(item));
        }

        return collection;
    }
    
    protected async Task<TEntity?> MapAsync(TData? data)
    {
        if (data is null)
        {
            return default;
        }

        return await MapperAsync(data);
    }
    
    protected abstract Task<TEntity> MapperAsync(TData data);
    
    
    public async Task<IEntityCollection<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var data = await _select.SelectAllAsync(cancellationToken);

        return await MapAsync(data);
    }

    public async Task<TEntity?> GetByIdAsync(Id<TEntity> id, CancellationToken cancellationToken = default)
    {
        var data = await _select.SelectByIdAsync(id.Value, cancellationToken);

        return await MapAsync(data);
    }
    
    protected async Task<TEntity> UpdateAsync(Id<TEntity> id, Action<TData> patch, CancellationToken cancellationToken)
    {
        var data = await _select.SelectByIdAsync(id.Value, cancellationToken);

        if (data is null)
        {
            throw new EntityNotFoundException<TEntity>(id);
        }
        
        patch(data);

        await _update.UpdateAsync(id.Value, data, cancellationToken);

        return (await GetByIdAsync(id, cancellationToken))!;
    }

    public async Task DeleteAsync(Id<TEntity> id, CancellationToken cancellationToken = default)
    {
        await _delete.DeleteAsync(id.Value, cancellationToken);
    }
    
    
}

public abstract class BaseService<TEntity, TData, TCreate> : BaseService<TEntity, TData> where TEntity : IEntity<TEntity>
{
    
    private readonly IRepositoryInsert<TData> _insert;

    public BaseService(IRepository<TData> repository) : base(repository)
    {
        _insert = repository;
    }

    public BaseService(IRepositorySelect<TData> select, IRepositoryInsert<TData> insert, IRepositoryUpdate<TData> update, IRepositoryDelete delete) : base(select, update, delete)
    {
        _insert = insert;
    }    
    
    protected abstract Task<(Id<TEntity>, TData)> CreateAsync(TCreate create);
    
    public async Task<TEntity> CreateAsync(TCreate create, CancellationToken cancellationToken)
    {
        var (id, data) = await CreateAsync(create);

        await _insert.InsertAsync(data, cancellationToken);

        return (await GetByIdAsync(id, cancellationToken))!;
    }
}