namespace TobyMeehan.Com.Data.SqlKata;

public class Token : IToken
{
    public string AccessToken { get; set; }
    public string TokenType { get; set; }
    public long ExpiresIn { get; set; }
    public string RefreshToken { get; set; }
}