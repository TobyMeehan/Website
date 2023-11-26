using TobyMeehan.Com.Data.Caching;

namespace TobyMeehan.Com.Data.Domain;

public abstract class BaseService<TEntity, TId, TData> where TEntity : IEntity<TId>
{
    protected BaseService(ICacheService<TData, Id<TId>> cache)
    {
        Cache = cache;
    }

    protected ICacheService<TData, Id<TId>> Cache { get; }

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
    
    protected abstract Task<TEntity> MapAsync(TData data);
}

public abstract class BaseService<TEntity, TDto> : BaseService<TEntity, TEntity, TDto> where TEntity : IEntity<TEntity>
{
    protected BaseService(ICacheService<TDto, Id<TEntity>> cache) : base(cache)
    {
    }
}