using SqlKata;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using TobyMeehan.Com.Data.Configuration;
using TobyMeehan.Com.Data.Extensions;
using TobyMeehan.Com.Data.Security;

namespace TobyMeehan.Com.Data.SqlKata
{
    public class OAuthSessionRepository : RepositoryBase<IOAuthSession, OAuthSession, NewOAuthSession>, IOAuthSessionRepository
    {
        private readonly ITokenProvider _tokenProvider;
        private readonly IConnectionRepository _connections;
        private readonly OAuthOptions _options;

        public OAuthSessionRepository(QueryFactory queryFactory, IIdGenerator idGenerator, ITokenProvider tokenProvider, IConnectionRepository connections, IOptions<OAuthOptions> options) : base(queryFactory, idGenerator, "oauthsessions")
        {
            _tokenProvider = tokenProvider;
            _connections = connections;
            _options = options.Value;
        }

        protected override Query Query()
        {
            return base.Query()
                .From(Table);
        }

        protected override async Task<IReadOnlyList<IOAuthSession>> MapAsync(IEnumerable<OAuthSession> items)
        {
            var list = items.ToList();
            
            foreach (var item in list)
            {
                item.Connection = await _connections.GetByIdAsync(item.ConnectionId);
            }

            return await base.MapAsync(list);
        }

        public async Task<IOAuthSession> GetByAuthCodeAsync(string authCode)
        {
            return await SelectSingleAsync(query => query.Where("AuthorizationCode", authCode));
        }

        public async Task<IOAuthSession> GetByRefreshTokenAsync(string refreshToken)
        {
            return await SelectSingleAsync(query => query.Where("RefreshToken", refreshToken));
        }

        private string GenerateRefreshToken()
        {
            using var rng = RandomNumberGenerator.Create();

            var data = new byte[32];
            rng.GetBytes(data);

            return Convert.ToBase64String(data);
        }
        
        public async Task<IToken> GenerateToken(IOAuthSession session)
        {
            var refreshToken = GenerateRefreshToken();

            await QueryFactory.Query(Table).Where("Id", session.Id.Value).UpdateAsync(new
            {
                RefreshToken = refreshToken,
                Expiry = DateTime.UtcNow + _options.RefreshTokenExpiry,
                AuthorizationCode = ""
            });
            
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, session.Connection.User.Id.Value),
                new(ClaimTypes.Name, session.Connection.User.Username),
                new(ClaimTypes.Actor, session.Connection.Application.Id.Value)
            };
            
            claims.AddRange(session.Connection.User.Roles.Select(role => new Claim(ClaimTypes.Role, role.Name)));
            claims.AddRange(session.Scopes.Select(scope => new Claim("scp", scope)));

            var expiry = DateTime.UtcNow + _options.TokenExpiry;
            
            return new Token
            {
                AccessToken = _tokenProvider.CreateToken(claims, expiry),
                ExpiresIn = (long)(expiry - DateTime.UtcNow).TotalSeconds,
                RefreshToken = session.RefreshToken
            };
        }

        public async Task DeleteByConnectionAsync(Id<IConnection> connectionId)
        {
            await QueryFactory.Query(Table).Where("ConnectionId", connectionId.Value).DeleteAsync();
        }
    }
}
