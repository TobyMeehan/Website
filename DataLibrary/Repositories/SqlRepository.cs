using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<IList<T>> GetAsync()
        {
            return (await _table.SelectAsync()).ToList();
        }

        public async Task<T> GetByIdAsync(string id)
        {
            return (await _table.SelectByAsync(x => x.Id == id)).SingleOrDefault();
        }

        public async Task DeleteAsync(string id)
        {
            await _table.DeleteAsync(x => x.Id == id);
        }
    }
}
