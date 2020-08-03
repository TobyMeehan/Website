using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Sql;

namespace TobyMeehan.Com.Data.Repositories
{
    public class SqlRepository<T> where T : EntityBase
    {
        private readonly ISqlTable<T> _table;

        public SqlRepository(ISqlTable<T> table)
        {
            _table = table;
        }

        public virtual async Task<IList<T>> GetAsync()
        {
            return (await SelectAsync()).ToList();
        }

        public virtual async Task<T> GetByIdAsync(string id)
        {
            return (await SelectAsync(x => x.Id == id)).SingleOrDefault();
        }

        public virtual async Task DeleteAsync(string id)
        {
            await _table.DeleteAsync(x => x.Id == id);
        }

        protected virtual Task<IEnumerable<T>> FormatAsync(IEnumerable<T> values)
        {
            return Task.FromResult(values);
        }

        protected async Task<IEnumerable<T>> SelectAsync()
        {
            return await FormatAsync(await _table.SelectAsync());
        }

        protected async Task<IEnumerable<T>> SelectAsync(Expression<Predicate<T>> expression)
        {
            return await FormatAsync(await _table.SelectByAsync(expression));
        }

        protected async Task<IEnumerable<T>> SelectAsync<TForeign>(Expression<Func<T, TForeign, bool>> expression)
        {
            return await FormatAsync(await _table.SelectByAsync(expression));
        }
    }
}
