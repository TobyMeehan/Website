using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Collections;
using TobyMeehan.Com.Data.Models;

namespace TobyMeehan.Com.Data.Repositories
{
    public interface IOAuthSessionRepository
    {
        Task<OAuthSession> AddAsync(string connectionId, string redirectUri, string scope, string codeChallenge, DateTime? expiry = null);

        Task<IEntityCollection<OAuthSession>> GetAsync();

        Task<OAuthSession> GetByIdAsync(string id);

        Task<OAuthSession> GetByAuthCodeAsync(string authCode);

        Task<OAuthSession> GetByRefreshTokenAsync(string refreshToken);

        Task<WebToken> GenerateToken(OAuthSession session);

        Task DeleteAsync(string id);

        Task DeleteByConnectionAsync(string connectionId);
    }
}
