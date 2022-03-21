namespace TobyMeehan.Com.Data;

public interface IToken
{
    string AccessToken { get; }
    string TokenType { get; }
    long ExpiresIn { get; }
    string RefreshToken { get; }
}