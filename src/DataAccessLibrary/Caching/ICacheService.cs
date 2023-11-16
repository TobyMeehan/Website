using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Data.Caching;

public interface ICacheService<T, TKey>
{
    T? Get(TKey key);

    T? Get(Func<T, bool> predicate);

    void Set(TKey key, T value);

    void Remove(TKey key);

    void RemoveWhere(Func<T, bool> predicate);
}