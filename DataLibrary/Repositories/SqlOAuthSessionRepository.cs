using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Extensions;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Security;
using TobyMeehan.Sql;

namespace TobyMeehan.Com.Data.Repositories
{
    public class SqlOAuthSessionRepository : SqlRepository<OAuthSession>, IOAuthSessionRepository
    {
        private readonly ISqlTable<OAuthSession> _table;
        private readonly ITokenProvider _tokenProvider;
        private readonly IConnectionRepository _connections;

        public SqlOAuthSessionRepository(ISqlTable<OAuthSession> table, ITokenProvider tokenProvider, IConnectionRepository connections) : base(table)
        {
            _table = table;
            _tokenProvider = tokenProvider;
            _connections = connections;
        }

        protected override async Task<IEnumerable<OAuthSession>> FormatAsync(IEnumerable<OAuthSession> values)
        {
            foreach (var session in values)
            {
                session.Connection = await _connections.GetByIdAsync(session.ConnectionId);
            }

            return values;
        }

        public async Task<OAuthSession> AddAsync(string connectionId, string redirectUri, string scope, string codeChallenge, DateTime? expiry = null)
        {
            string id = Guid.NewGuid().ToString();

            await _table.InsertAsync(new
            {
                Id = id,
                ConnectionId = connectionId,
                AuthorizationCode = Guid.NewGuid().ToToken(),
                RedirectUri = redirectUri,
                Scope = scope,
                CodeChallenge = codeChallenge,
                Expiry = expiry ?? DateTime.Now.AddMinutes(30)
            });

            return (await SelectAsync(s => s.Id == id)).Single();
        }

        public Task DeleteByConnectionAsync(string connectionId)
        {
            return _table.DeleteAsync(s => s.ConnectionId == connectionId);
        }

        public async Task<WebToken> GenerateToken(OAuthSession session)
        {
            session.RefreshToken = Guid.NewGuid().ToToken();

            await _table.UpdateAsync(s => s.Id == $"{session.Id}", new
            {
                session.RefreshToken,
                Expiry = DateTime.UtcNow.AddMonths(6),
                AuthorizationCode = ""
            });

            DateTime expiry = DateTime.UtcNow.AddDays(1);

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, session.Connection.User.Id),
                new Claim(ClaimTypes.Name, session.Connection.User.Username),
                new Claim(ClaimTypes.Actor, session.Connection.Application.Id)
            };

            foreach (var role in session.Connection.User.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, $"User:{role.Name}"));
            }

            foreach (var scope in session.Scopes)
            {
                claims.Add(new Claim(ClaimTypes.Role, $"Scope:{scope}"));
            }

            return new WebToken
            {
                AccessToken = _tokenProvider.CreateToken(claims, expiry),
                ExpiresIn = expiry.ToBinary(),
                RefreshToken = session.RefreshToken
            };
        }

        public async Task<OAuthSession> GetByAuthCodeAsync(string authCode)
        {
            return (await SelectAsync(s => s.AuthorizationCode == authCode)).Single();
        }

        public async Task<OAuthSession> GetByRefreshTokenAsync(string refreshToken)
        {
            return (await SelectAsync(s => s.RefreshToken == refreshToken)).Single();
        }
    }
}
