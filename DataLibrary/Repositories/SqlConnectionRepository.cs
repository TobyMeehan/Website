using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Security;
using TobyMeehan.Sql;

namespace TobyMeehan.Com.Data.Repositories
{
    public class SqlConnectionRepository : SqlRepository<Connection>, IConnectionRepository
    {
        private readonly ISqlTable<Connection> _table;
        private readonly ISqlTable<AuthorizationCode> _authCodeTable;

        public SqlConnectionRepository(ISqlTable<Connection> table, ISqlTable<AuthorizationCode> authCodeTable) : base(table)
        {
            _table = table;
            _authCodeTable = authCodeTable;
        }

        private async Task<Connection> InsertConnectionAsync(string userId, string appId, string codeChallenge)
        {
            string id = Guid.NewGuid().ToString();

            await _table.InsertAsync(new
            {
                Id = id,
                UserId = userId,
                AppId = appId,
                CodeChallenge = codeChallenge
            });

            return await GetByIdAsync(id);
        }

        public async Task<AuthorizationCode> AddAsync(string userId, string appId, string codeChallenge = null)
        {
            Connection connection = await GetByUserAndApplicationAsync(userId, appId);

            if (connection == null)
            {
                connection = await InsertConnectionAsync(userId, appId, codeChallenge);
            }

            string id = Guid.NewGuid().ToString();

            await _authCodeTable.InsertAsync(new
            {
                Id = id,
                ConnectionId = connection.Id,
                Expiry = DateTime.Now.AddMinutes(30),
                Code = RandomString.GenerateCrypto()
            });

            return (await _authCodeTable.SelectByAsync(ac => ac.Id == id)).SingleOrDefault();
        }

        public async Task<IList<Connection>> GetByApplicationAsync(string appId)
        {
            return (await _table.SelectByAsync(c => c.AppId == appId)).ToList();
        }

        public async Task<AuthorizationCode> GetByAuthorizationCodeAsync(string code)
        {
            return (await _authCodeTable.SelectByAsync(ac => ac.Code == code)).SingleOrDefault();
        }

        public async Task<Connection> GetByUserAndApplicationAsync(string userId, string appId)
        {
            return (await _table.SelectByAsync(c => c.UserId == userId && c.AppId == appId)).SingleOrDefault();
        }

        public async Task<IList<Connection>> GetByUserAsync(string userId)
        {
            return (await _table.SelectByAsync(c => c.UserId == userId)).ToList();
        }
    }
}
