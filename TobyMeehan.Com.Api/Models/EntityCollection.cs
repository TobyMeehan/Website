using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Api.Models
{
    public class EntityCollection<T> : IEnumerable<T> where T : EntityModel
    {
        public List<T> Items { get; set; }

        public T this[string id] => Items.Single(i => i.Id == id);

        public IEnumerator<T> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        public bool TryGetItem(string id, out T entity)
        {
            entity = Items.FirstOrDefault(i => i.Id == id);

            return entity != null;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
