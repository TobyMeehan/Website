using System;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Data;

public interface IOAuthSessionRepository
{
    Task<IOAuthSession> GetByAuthCodeAsync(string authCode);

    Task<IOAuthSession> GetByRefreshTokenAsync(string refreshToken);

    Task<IOAuthSession> AddAsync(Action<NewOAuthSession> session);
    
    Task<IToken> GenerateToken(IOAuthSession session);

    Task DeleteAsync(Id<IOAuthSession> id);

    Task DeleteByConnectionAsync(Id<IConnection> connectionId);
}

public class NewOAuthSession
{
    public Id<IConnection> ConnectionId { get; set; }
    public string RedirectUri { get; set; }
    public string Scope { get; set; } = null;
    public string CodeChallenge { get; set; }
    public DateTime? Expiry { get; set; }
}