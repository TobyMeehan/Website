using System;
using System.Collections.Generic;
using System.Linq;
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
            try
            {
                item = value.Single();
                return true;
            }
            catch (Exception)
            {
                item = default;
                return false;
            }
        }
    }
}
