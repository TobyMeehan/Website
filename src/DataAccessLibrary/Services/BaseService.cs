using TobyMeehan.Com.Data.Entities;
using TobyMeehan.Com.Data.Repositories;
using TobyMeehan.Com.Data.Repositories.Models;

namespace TobyMeehan.Com.Data.Services;

public abstract class BaseService<TEntity, TData, TCreate> where TEntity : IEntity<TEntity>
{
    private readonly IRepositorySelect<TData> _select;
    private readonly IRepositoryInsert<TData> _insert;
    private readonly IRepositoryUpdate<TData> _update;
    private readonly IRepositoryDelete _delete;

    public BaseService(IRepository<TData> repository)
    {
        _select = repository;
        _insert = repository;
        _update = repository;
        _delete = repository;
    }
    
    public BaseService(IRepositorySelect<TData> select, IRepositoryInsert<TData> insert, IRepositoryUpdate<TData> update, IRepositoryDelete delete)
    {
        _select = select;
        _insert = insert;
        _update = update;
        _delete = delete;
    }
    
    protected async Task<IEntityCollection<TEntity>> MapAsync(IEnumerable<TData> data)
    {
        EntityCollection<TEntity> collection = new();

        foreach (var item in data)
        {
            collection.Add(await MapAsync(item));
        }

        return collection;
    }

    protected abstract Task<TEntity> MapAsync(TData data);

    public async Task<IEntityCollection<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var data = await _select.SelectAllAsync(cancellationToken);

        return await MapAsync(data);
    }

    public async Task<TEntity> GetByIdAsync(Id<TEntity> id, CancellationToken cancellationToken = default)
    {
        var data = await _select.SelectByIdAsync(id.Value, cancellationToken);

        return await MapAsync(data);
    }

    protected abstract Task<(Id<TEntity>, TData)> CreateAsync(TCreate create);
    
    public async Task<TEntity> CreateAsync(TCreate create, CancellationToken cancellationToken)
    {
        var (id, data) = await CreateAsync(create);

        await _insert.InsertAsync(data, cancellationToken);

        return await GetByIdAsync(id, cancellationToken);
    }

    protected async Task<TEntity> UpdateAsync(Id<TEntity> id, Action<TData> patch, CancellationToken cancellationToken)
    {
        var data = await _select.SelectByIdAsync(id.Value, cancellationToken);

        patch(data);

        await _update.UpdateAsync(id.Value, data, cancellationToken);

        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task DeleteAsync(Id<TEntity> id, CancellationToken cancellationToken = default)
    {
        await _delete.DeleteAsync(id.Value, cancellationToken);
    }
}