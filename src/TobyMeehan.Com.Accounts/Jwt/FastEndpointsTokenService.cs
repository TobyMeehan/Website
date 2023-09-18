using FastEndpoints.Security;
using Microsoft.Extensions.Options;
using TobyMeehan.Com.Accounts.Configuration;

namespace TobyMeehan.Com.Accounts.Jwt;

public class FastEndpointsTokenService : ITokenService
{
    private readonly AuthenticationOptions _options;

    public FastEndpointsTokenService(IOptions<AuthenticationOptions> options)
    {
        _options = options.Value;
    }
    
    public Task<JsonWebToken> GenerateTokenAsync(ISession session)
    {
        string token = JWTBearer.CreateToken(
            signingKey: _options.Jwt.TokenSigningKey,
            expireAt: DateTime.UtcNow + _options.Jwt.Expiry,
            audience: session.Application.Id.Value,
            privileges: u =>
            {
                u["sub"] = session.User.Id.Value;
                u["SessionId"] = session.Id.Value;
            });

        return Task.FromResult(new JsonWebToken(token, "Bearer", session.Expiry, session.RefreshToken));
    }
}