namespace TobyMeehan.Com.Accounts.Jwt;

public interface ITokenService
{
    Task<JsonWebToken> GenerateTokenAsync(ISession session);
}