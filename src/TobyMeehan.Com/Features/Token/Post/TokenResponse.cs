namespace TobyMeehan.Com.Features.Token.Post;

public class TokenResponse
{
    public required string Token { get; set; }
    public int ExpiresIn { get; set; }
}