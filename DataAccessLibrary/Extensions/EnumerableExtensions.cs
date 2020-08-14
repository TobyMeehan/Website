using System;
using System.Collections.Generic;
using System.Text;
using TobyMeehan.Com.Data.Models;

namespace TobyMeehan.Com.Data.Extensions
{
    public static class EnumerableExtensions
    {
        public static EntityCollection<T> ToEntityCollection<T>(this IEnumerable<T> list) where T : EntityBase
        {
            EntityCollection<T> collection = new EntityCollection<T>();

            foreach (var item in list)
            {
                collection.Add(item);
            }

            return collection;
        }
    }
}
