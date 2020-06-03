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
    public class SqlRepository<T> : IRepository<T> where T : EntityBase
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

        public Task<IEnumerable<T>> GetByAsync<U>(Expression<Func<T, U, bool>> expression)
        {
            return _table.SelectByAsync(expression);
        }

        public Task<T> GetByIdAsync(string id)
        {
            return GetSingleByAsync(x => x.Id == id);
        }

        public async Task<T> GetSingleByAsync(Expression<Predicate<T>> expression)
        {
            return (await GetByAsync(expression)).Single();
        }

        public async Task<T> GetSingleByAsync<U>(Expression<Func<T, U, bool>> expression)
        {
            return (await _table.SelectByAsync(expression)).Single();
        }

        public virtual Task RemoveByAsync(Expression<Predicate<T>> expression)
        {
            return _table.DeleteAsync(expression);
        }

        public Task RemoveByIdAsync(string id)
        {
            return RemoveByAsync(x => x.Id == id);
        }

        public Task UpdateByAsync(Expression<Predicate<T>> expression, object value)
        {
            return _table.UpdateAsync(expression, value);
        }

        public Task UpdateByIdAsync(string id, object value)
        {
            return UpdateByAsync(x => x.Id == id, value);
        }
    }
}
