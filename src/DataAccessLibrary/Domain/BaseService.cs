using TobyMeehan.Com.Data.Caching;

namespace TobyMeehan.Com.Data.Domain;

public abstract class BaseService<TEntity, TData> where TEntity : IEntity<TEntity>
{
    protected BaseService(ICacheService<TData, Id<TEntity>> cache)
    {
        Cache = cache;
    }

    protected ICacheService<TData, Id<TEntity>> Cache { get; }

    protected async IAsyncEnumerable<TEntity> GetAsync(IAsyncEnumerable<TData> data)
    {
        await foreach (var item in data)
        {
            yield return await MapAsync(item);
        }
    }

    protected async IAsyncEnumerable<TEntity> GetAsync(IEnumerable<TData> data)
    {
        foreach (var item in data)
        {
            yield return await MapAsync(item);
        }
    }

    protected async Task<TEntity> GetAsync(TData data)
    {
        var entity = await MapAsync(data);

        Cache.Set(entity.Id, data);

        return entity;
    }

    protected async Task<T> GetAsync<T>(TData data) where T : TEntity
    {
        return (T) await GetAsync(data);
    }
    
    
    protected abstract Task<TEntity> MapAsync(TData data);
}