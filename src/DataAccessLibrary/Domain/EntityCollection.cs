using System.Collections;
using OneOf;
using TobyMeehan.Com.Results;

namespace TobyMeehan.Com.Data.Domain;

public class EntityCollection<T, TId> : IEntityCollection<T, TId> where T : IEntity<TId>
{
    public EntityCollection() { }

    public EntityCollection(IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            _items.Add(item.Id, item);
        }
    }
    
    private readonly Dictionary<Id<TId>, T> _items = new();
    
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

    public OneOf<T, NotFound> this[Id<TId> id] => _items.TryGetValue(id, out var entity) ? entity : new NotFound();

    public T? Find(string str)
    {
        var id = new Id<TId>(str);

        return _items.TryGetValue(id, out var entity) ? entity : default;
    }
}

public class EntityCollection<T> : EntityCollection<T, T>, IEntityCollection<T> where T : IEntity<T>
{
    public EntityCollection() { }

    public EntityCollection(IEnumerable<T> items) : base(items) { }
}