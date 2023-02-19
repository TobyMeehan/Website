using System.Collections;

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

    public T? this[Id<T> id] => _items.ContainsKey(id) ? _items[id] : default;
}