namespace TobyMeehan.Com.Accounts.Jwt;

public class JsonWebToken
{
    public JsonWebToken(string accessToken, string tokenType, DateTime expiry, string? refreshToken = null)
    {
        AccessToken = accessToken;
        TokenType = tokenType;
        Expiry = expiry;
        RefreshToken = refreshToken;
    }
    
    public string AccessToken { get; set; }
    public string TokenType { get; set; }
    public DateTime Expiry { get; set; }
    public string? RefreshToken { get; set; }
}