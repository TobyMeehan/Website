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
    public class SqlRepository<T> : IRepository<T>
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

        public async Task<T> GetBySingleAsync(Expression<Predicate<T>> expression)
        {
            return (await GetByAsync(expression)).Single();
        }

        public virtual Task RemoveByAsync(Expression<Predicate<T>> expression)
        {
            return _table.DeleteAsync(expression);
        }

        public Task UpdateByAsync(Expression<Predicate<T>> expression, T value)
        {
            return _table.UpdateAsync(expression, value);
        }
    }
}
