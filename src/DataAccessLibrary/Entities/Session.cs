namespace TobyMeehan.Com.Data.Entities;

public class Session : Entity<ISession>, ISession
{
    public Session(string id, string connectionId, string redirectId, string code, IEnumerable<string> scope, string? codeChallenge, string? refreshToken, DateTime expiry) : base(id)
    {
        ConnectionId = new Id<IConnection>(connectionId);
        RedirectId = new Id<IRedirect>(redirectId);
        AuthorizationCode = code;
        Scope = scope;
        CodeChallenge = codeChallenge;
        RefreshToken = refreshToken;
        Expiry = expiry;
    }

    public Id<IConnection> ConnectionId { get; }
    public Id<IRedirect> RedirectId { get; }
    public string AuthorizationCode { get; }
    public IEnumerable<string> Scope { get; }
    public string? CodeChallenge { get; }
    public string? RefreshToken { get; }
    public DateTime Expiry { get; }
}