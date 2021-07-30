using SqlKata;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Collections;
using TobyMeehan.Com.Data.Extensions;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories;
using TobyMeehan.Com.Data.Security;

namespace TobyMeehan.Com.Data.SqlKata
{
    public class OAuthSessionRepository : RepositoryBase<OAuthSession>, IOAuthSessionRepository
    {
        private readonly Func<QueryFactory> _queryFactory;
        private readonly ITokenProvider _tokenProvider;
        private readonly IConnectionRepository _connections;

        public OAuthSessionRepository(Func<QueryFactory> queryFactory, ITokenProvider tokenProvider, IConnectionRepository connections) : base(queryFactory)
        {
            _queryFactory = queryFactory;
            _tokenProvider = tokenProvider;
            _connections = connections;
        }

        protected override Query Query()
        {
            return base.Query()
                .From("oauthsessions");
        }

        protected override async Task<IEntityCollection<OAuthSession>> MapAsync(IEnumerable<OAuthSession> items)
        {
            foreach (var item in items)
            {
                item.Connection = await _connections.GetByIdAsync(item.ConnectionId);
            }

            return await base.MapAsync(items);
        }



        public async Task<OAuthSession> AddAsync(string connectionId, string redirectUri, string scope, string codeChallenge, DateTime? expiry = null)
        {
            using (QueryFactory db = _queryFactory.Invoke())
            {
                string id = await db.Query("oauthsessions").InsertGetIdAsync<string>(new
                {
                    Id = Guid.NewGuid().ToString(),
                    ConnectionId = connectionId,
                    AuthorizationCode = Guid.NewGuid().ToToken(),
                    RedirectUri = redirectUri,
                    Scope = scope,
                    CodeChallenge = codeChallenge,
                    Expiry = expiry ?? DateTime.Now.AddMinutes(30)
                });

                return await GetByIdAsync(id);
            }
        }



        public async Task<IEntityCollection<OAuthSession>> GetAsync()
        {
            return await SelectAsync();
        }

        public async Task<OAuthSession> GetByAuthCodeAsync(string authCode)
        {
            return await SelectSingleAsync(query => query.Where("AuthorizationCode", authCode));
        }

        public async Task<OAuthSession> GetByIdAsync(string id)
        {
            return await SelectSingleAsync(query => query.Where("oauthsessions.Id", id));
        }

        public async Task<OAuthSession> GetByRefreshTokenAsync(string refreshToken)
        {
            return await SelectSingleAsync(query => query.Where("RefreshToken", refreshToken));
        }



        public async Task DeleteAsync(string id)
        {
            using (QueryFactory db = _queryFactory.Invoke())
            {
                await db.Query("oauthsessions").Where("Id", id).DeleteAsync();
            }
        }

        public async Task DeleteByConnectionAsync(string connectionId)
        {
            using (QueryFactory db = _queryFactory.Invoke())
            {
                await db.Query("oauthsessions").Where("ConnectionId", connectionId).DeleteAsync();
            }
        }



        public async Task<WebToken> GenerateToken(OAuthSession session)
        {
            session.RefreshToken = Guid.NewGuid().ToToken();

            using (QueryFactory db = _queryFactory.Invoke())
            {
                await db.Query("oauthsessions").Where("Id", session.Id).UpdateAsync(new
                {
                    session.RefreshToken,
                    Expiry = DateTime.UtcNow.AddMonths(6),
                    AuthorizationCode = ""
                });
            }

            DateTime expiry = DateTime.UtcNow.AddDays(1);

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, session.Connection.User.Id),
                new Claim(ClaimTypes.Name, session.Connection.User.Username),
                new Claim(ClaimTypes.Actor, session.Connection.Application.Id)
            };

            foreach (var role in session.Connection.User.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }

            foreach (var scope in session.Scopes)
            {
                claims.Add(new Claim("scp", scope));
            }

            return new WebToken
            {
                AccessToken = _tokenProvider.CreateToken(claims, expiry),
                ExpiresIn = (long)(expiry - DateTime.Now).TotalSeconds,
                RefreshToken = session.RefreshToken
            };
        }
    }
}
