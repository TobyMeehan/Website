using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Api.Models
{
    public class EntityCollection<T> where T : EntityModel
    {
        private List<T> _items = new List<T>();

        public T this[string id] => _items.Single(i => i.Id == id);

        public bool TryGetItem(string id, out T entity)
        {
            entity = _items.FirstOrDefault(i => i.Id == id);

            return entity != null;
        }
    }
}
