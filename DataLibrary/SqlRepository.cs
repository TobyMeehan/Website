using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Sql;

namespace TobyMeehan.Com.Data
{
    public class SqlRepository<T> : IRepository<T> where T : IEntity
    {
        private readonly ISqlTable<T> _table;

        public SqlRepository(ISqlTable<T> table)
        {
            _table = table;
        }

        public virtual Task AddAsync(object value)
        {
            return _table.InsertAsync(value);
        }

        public virtual Task<IEnumerable<T>> GetAsync()
        {
            return _table.SelectAsync();
        }

        public virtual Task<IEnumerable<T>> GetByAsync(Expression<Predicate<T>> expression)
        {
            return _table.SelectByAsync(expression);
        }

        public virtual async Task<T> GetByIdAsync(string id)
        {
            return (await _table.SelectByAsync(x => x.Id == id)).Single();
        }

        public virtual Task RemoveAsync(Expression<Predicate<T>> expression)
        {
            return _table.DeleteAsync(expression);
        }

        public virtual Task RemoveByIdAsync(string id)
        {
            return _table.DeleteAsync(x => x.Id == id);
        }

        public virtual Task UpdateAsync(T value)
        {
            return _table.UpdateAsync(x => x.Id == value.Id, value);
        }
    }
}
