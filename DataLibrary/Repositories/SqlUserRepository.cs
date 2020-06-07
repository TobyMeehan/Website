using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Sql;

namespace TobyMeehan.Com.Data.Repositories
{
    public class SqlUserRepository : SqlRepository<User>, IUserRepository
    {
        private readonly ISqlTable<User> _table;

        public SqlUserRepository(ISqlTable<User> table) : base(table)
        {
            _table = table;
        }

        public async Task<User> AddAsync(string username, string password)
        {
            string id = Guid.NewGuid().ToString();

            await _table.InsertAsync(new
            {
                Id = id,
                Username = username,
                Password = password
            });

            return await GetByIdAsync(id);
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            return (await _table.SelectByAsync(x => x.Username == username)).SingleOrDefault();
        }

        public Task UpdatePasswordAysnc(string id, string password)
        {
            return _table.UpdateAsync(x => x.Id == id, new
            {
                Password = password
            });
        }

        public Task UpdateUsernameAsync(string id, string username)
        {
            return _table.UpdateAsync(x => x.Id == id, new
            {
                Username = username
            });
        }
    }
}
