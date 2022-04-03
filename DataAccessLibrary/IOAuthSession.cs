using System;
using System.Collections;
using System.Collections.Generic;

namespace TobyMeehan.Com.Data;

public interface IOAuthSession
{
    Id<IOAuthSession> Id { get; }
    Id<IConnection> ConnectionId { get; }
    IConnection Connection { get; }
    
    string AuthorizationCode { get; }
    string RedirectUri { get; }
    string CodeChallenge { get; }
    IEnumerable<string> Scopes { get; }
    
    string RefreshToken { get; }
    DateTime Expiry { get; }
}