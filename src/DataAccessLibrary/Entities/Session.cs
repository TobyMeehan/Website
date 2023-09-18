namespace TobyMeehan.Com.Data.Entities;

public class Session : Entity<ISession>, ISession
{
    public Session(string id, IConnection connection, IRedirect? redirect, IEnumerable<string> scope, string? refreshToken, DateTime expiry) : base(id)
    {
        Connection = connection;
        Redirect = redirect;
        Scope = scope;
        RefreshToken = refreshToken;
        Expiry = expiry;
    }

    public IConnection Connection { get; }
    public IRedirect? Redirect { get; }
    public IEnumerable<string> Scope { get; }
    public string? RefreshToken { get; }
    public DateTime Expiry { get; }
}