using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TobyMeehan.Com.Data.Models;

namespace TobyMeehan.Com.Data.Collections
{
    public class EntityCollection<T> : IEntityCollection<T> where T : EntityBase
    {
        public EntityCollection() { }

        public EntityCollection(IEnumerable<T> collection)
        {
            _items = collection.ToList();
        }

        private List<T> _items = new List<T>();

        public T this[string id] => _items.First(i => i.Id == id);

        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
