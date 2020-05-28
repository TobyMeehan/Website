using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Data.Models
{
    public abstract class EntityBase
    {
        public string Id { get; set; }

        public override bool Equals(object obj)
        {
            if (this == null && obj == null)
            {
                return false;
            }

            return obj as EntityBase == this;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(EntityBase x, EntityBase y)
        {
            return x?.Id == y?.Id;
        }

        public static bool operator !=(EntityBase x, EntityBase y)
        {
            return !(x == y);
        }
    }

    public static class EntityBaseExtensions
    {
        public static IEnumerable<T> DistinctEntities<T>(this IEnumerable<T> entities) where T : EntityBase
        {
            return entities.GroupBy(e => e.Id).Select(g => g.First());
        }
    }
}
