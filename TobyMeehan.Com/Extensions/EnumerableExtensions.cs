using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Extensions
{
    public static class EnumerableExtensions
    {
        public static int IndexOf<T>(this IEnumerable<T> source, T value)
        {
            int index = 0;

            foreach (T item in source)
            {
                if (item.Equals(value))
                {
                    return index;
                }

                index++;
            }

            return -1;
        }
    }
}
