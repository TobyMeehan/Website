using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Sql;

namespace TobyMeehan.Com.Data.Repositories
{
    public class SqlRoleRepository : SqlRepository<Role>, IRoleRepository
    {
        private readonly ISqlTable<Role> _table;

        public SqlRoleRepository(ISqlTable<Role> table) : base(table)
        {
            _table = table;
        }

        public async Task<Role> AddAsync(string name)
        {
            string id = Guid.NewGuid().ToString();

            await _table.InsertAsync(new
            {
                Id = id,
                Name = name
            });

            return await GetByIdAsync(id);
        }
    }
}
