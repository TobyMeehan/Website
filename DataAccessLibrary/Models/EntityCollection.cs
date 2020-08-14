using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TobyMeehan.Com.Data.Models
{
    public class EntityCollection<T> : IEnumerable<T> where T : EntityBase
    {
        public List<T> Items { get; set; } = new List<T>();

        public T this[string id] => Items.Single(i => i.Id == id);

        public bool TryGetItem(string id, out T entity)
        {
            entity = Items.FirstOrDefault(i => i.Id == id);

            return entity != null;
        }

        public void Add(T entity) => Items.Add(entity);

        public IEnumerator<T> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
