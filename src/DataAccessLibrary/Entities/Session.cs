namespace TobyMeehan.Com.Data.Entities;

public class Session : Entity<ISession>, ISession
{
    public Session(string id, IConnection connection, IRedirect? redirect, string code, IEnumerable<string> scope, string? codeChallenge, string? refreshToken, DateTime expiry) : base(id)
    {
        Connection = connection;
        Redirect = redirect;
        AuthorizationCode = code;
        Scope = scope;
        CodeChallenge = codeChallenge;
        RefreshToken = refreshToken;
        Expiry = expiry;
    }

    public IConnection Connection { get; }
    public IRedirect? Redirect { get; }
    public string AuthorizationCode { get; }
    public IEnumerable<string> Scope { get; }
    public string? CodeChallenge { get; }
    public string? RefreshToken { get; }
    public DateTime Expiry { get; }
}