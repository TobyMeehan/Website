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
    public class SqlOAuthSessionRepository : SqlRepository<OAuthSession>, IOAuthSessionRepository
    {
        private readonly ISqlTable<OAuthSession> _table;

        public SqlOAuthSessionRepository(ISqlTable<OAuthSession> table) : base(table)
        {
            _table = table;
        }

        public async Task<OAuthSession> AddAsync(string connectionId, string codeChallenge, DateTime expiry)
        {
            string id = Guid.NewGuid().ToString();

            await _table.InsertAsync(new
            {
                Id = id,
                ConnectionId = connectionId,
                AuthorizationCode = RandomString.GenerateCrypto(),
                CodeChallenge = codeChallenge,
                Expiry = DateTime.Now.AddMinutes(30)
            });

            return (await _table.SelectByAsync(s => s.Id == id)).Single();
        }

        public Task DeleteByConnectionAsync(string connectionId)
        {
            return _table.DeleteAsync(s => s.ConnectionId == connectionId);
        }

        public async Task<OAuthSession> GetByAuthCodeAsync(string authCode)
        {
            return (await _table.SelectByAsync(s => s.AuthorizationCode == authCode)).Single();
        }

        public async Task<OAuthSession> GetByRefreshTokenAsync(string refreshToken)
        {
            return (await _table.SelectByAsync(s => s.RefreshToken == refreshToken)).Single();
        }
    }
}
