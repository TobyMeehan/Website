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
        private readonly ISqlTable<UserRole> _userRoleTable;
        private readonly IPasswordHash _passwordHash;

        public SqlUserRepository(ISqlTable<User> table, ISqlTable<UserRole> userRoleTable, IPasswordHash passwordHash) : base(table)
        {
            _table = table;
            _userRoleTable = userRoleTable;
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

        public Task AddRoleAsync(string id, string roleId)
        {
            return _userRoleTable.InsertAsync(new UserRole
            {
                UserId = id,
                RoleId = roleId
            });
        }

        public async Task<bool> AnyUsernameAsync(string username)
        {
            return (await GetByUsernameAsync(username)) != null;
        }

        public async Task<AuthenticationResult<User>> AuthenticateAsync(string username, string password)
        {
            User user = await GetByUsernameAsync(username);

            if (user == null)
            {
                return new AuthenticationResult<User>();
            }

            if (!_passwordHash.CheckPassword(password, user?.HashedPassword))
            {
                return new AuthenticationResult<User>();
            }

            return new AuthenticationResult<User>(user);
        }

        public async Task<IList<User>> GetByRoleAsync(string name)
        {
            return (await _table.SelectByAsync<Role>((u, r) => r.Name == name)).ToList();
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            return (await _table.SelectByAsync(x => x.Username == username)).SingleOrDefault();
        }

        public Task RemoveRoleAsync(string id, string roleId)
        {
            return _userRoleTable.DeleteAsync(x => x.UserId == id && x.RoleId == roleId);
        }

        public Task UpdatePasswordAysnc(string id, string password)
        {
            return _table.UpdateAsync(x => x.Id == id, new
            {
                HashedPassword = _passwordHash.HashPassword(password)
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
