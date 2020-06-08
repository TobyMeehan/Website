using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Security;
using TobyMeehan.Sql;

namespace TobyMeehan.Com.Data.Repositories
{
    public class SqlUserRepository : SqlRepository<User>, IUserRepository
    {
        private readonly ISqlTable<User> _table;
        private readonly IPasswordHash _passwordHash;

        public SqlUserRepository(ISqlTable<User> table, IPasswordHash passwordHash) : base(table)
        {
            _table = table;
            _passwordHash = passwordHash;
        }

        public async Task<User> AddAsync(string username, string password)
        {
            string id = Guid.NewGuid().ToString();

            await _table.InsertAsync(new
            {
                Id = id,
                Username = username,
                HashedPassword = _passwordHash.HashPassword(password)
            });

            return await GetByIdAsync(id);
        }

        public async Task<bool> AnyUsernameAsync(string username)
        {
            return (await GetByUsernameAsync(username)) != null;
        }

        public async Task<AuthenticationResult<User>> AuthenticateAsync(string username, string password)
        {
            User user = await GetByUsernameAsync(username);

            if (_passwordHash.CheckPassword(password, user?.HashedPassword))
            {
                return new AuthenticationResult<User>(user);
            }
            else
            {
                return new AuthenticationResult<User>();
            }
        }

        public async Task<IList<User>> GetByRoleAsync(string name)
        {
            return (await _table.SelectByAsync<Role>((u, r) => r.Name == name)).ToList();
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
