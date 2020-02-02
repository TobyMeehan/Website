using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLibrary.Data
{
    public class ProcessorBase
    {
        /// <summary>
        /// Validates list of items retrieved from SQL query and outputs the single item target.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool ValidateQuery<T>(List<T> value, out T item)
        {
            if (value.Count > 1)
            {
                item = default;
                return false;
            }
            else if (value.Count < 1 || value == null)
            {
                item = default;
                return false;
            }
            else
            {
                item = value[0];
                return true;
            }
        }

        /// <summary>
        /// Confirms whether an item exists in the provided query result. Does not check against multiple items.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool ValidateQuery<T>(List<T> value)
        {
            return value.Count > 0;
        }
    }
}
