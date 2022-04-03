using System;
using System.Collections.Generic;

namespace TobyMeehan.Com.Data.SqlKata;

public class OAuthSession : IOAuthSession
{
    public Id<IOAuthSession> Id { get; set; }
    
    public Id<IConnection> ConnectionId { get; set; }
    public IConnection Connection { get; set; }
    
    public string AuthorizationCode { get; set; }
    public string RedirectUri { get; set; }
    public string CodeChallenge { get; set; }
    public string Scope { get; set; }
    public IEnumerable<string> Scopes => Scope.Split(" ");
    public string RefreshToken { get; set; }
    public DateTime Expiry { get; set; }
}