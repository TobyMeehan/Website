using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TobyMeehan.Com.Data.Models
{
    public class EntityCollection<T> : IEnumerable<T> where T : EntityBase
    {
        private Dictionary<string, T> _items = new Dictionary<string, T>();

        public T this[string id] => _items[id];

        public bool TryGetItem(string id, out T entity) => _items.TryGetValue(id, out entity);

        public void Add(T entity) => _items.Add(entity.Id, entity);

        public IEnumerator<T> GetEnumerator()
        {
            return _items.Values.ToList().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
