using System.Collections;
using TobyMeehan.Com.Exceptions;

namespace TobyMeehan.Com.Data.Entities;

public class EntityCollection<T> : IEntityCollection<T> where T : IEntity<T>
{
    private readonly Dictionary<Id<T>, T> _items = new();
    
    public void Add(T entity)
    {
        _items.Add(entity.Id, entity);
    }
    
    public IEnumerator<T> GetEnumerator()
    {
        return _items.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public int Count => _items.Count;

    public T this[Id<T> id] => _items.TryGetValue(id, out var entity) ? entity : throw new EntityNotFoundException<T>(id);

    public T? Find(string str)
    {
        var id = new Id<T>(str);

        return _items.TryGetValue(id, out var entity) ? entity : default;
    }

    public static EntityCollection<T> Create(IEnumerable<T> items) => Create(items, x => x);

    public static EntityCollection<T> Create<TObject>(IEnumerable<TObject> items, Func<TObject, T> map)
    {
        var collection = new EntityCollection<T>();

        foreach (var item in items)
        {
            collection.Add(map.Invoke(item));
        }

        return collection;
    }
}