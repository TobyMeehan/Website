using System;

namespace TobyMeehan.Com.Data;

public interface IAuthorizationCode
{
    Id<IAuthorizationCode> Id { get; }
    Id<IConnection> ConnectionId { get; }
    
    IConnection Connection { get; }
    
    string Code { get; }
    string CodeChallenge { get; }
    DateTime Expiry { get; }
}