namespace TobyMeehan.Com.Accounts.Configuration;

public class AuthenticationOptions
{
    public JwtOptions Jwt { get; set; }
    
    public class JwtOptions
    {
        public string TokenSigningKey { get; set; }
    }
}