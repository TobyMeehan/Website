using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Data.Caching;

public class MemoryCacheService<T, TKey> : ICacheService<T, TKey> where TKey : notnull
{
    private readonly IMemoryCache _cache;
    private readonly (Type, Type) _key = (typeof(T), typeof(TKey));

    public MemoryCacheService(IMemoryCache cache)
    {
        _cache = cache;
    }
    
    public T? Get(TKey key)
    {
        var cache = _cache.Get<Dictionary<TKey, T>>(_key);

        return cache is null ? default : cache.GetValueOrDefault(key);
    }

    public T? Get(Func<T, bool> predicate)
    {
        var cache = _cache.Get<Dictionary<TKey, T>>(_key);

        if (cache is null)
        {
            return default;
        }

        return cache.Values
            .Where(predicate)
            .FirstOrDefault();
    }

    public void Set(TKey key, T value)
    {
        var cache = _cache.GetOrCreate(_key, _ => new Dictionary<TKey, T>());

        if (cache is null)
        {
            return;
        }
        
        cache[key] = value;
    }

    public void Remove(TKey key)
    {
        var cache = _cache.Get<Dictionary<TKey, T>>(_key);

        cache?.Remove(key);
    }

    public void RemoveWhere(Func<T, bool> predicate)
    {
        var cache = _cache.Get<Dictionary<TKey, T>>(_key);

        if (cache is null)
        {
            return;
        }

        cache = cache.Where(x => !predicate(x.Value))
            .ToDictionary(x => x.Key, x => x.Value);

        _cache.Set(_key, cache);
    }
}