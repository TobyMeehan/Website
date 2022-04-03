using System;

namespace TobyMeehan.Com.Data.Configuration;

public class OAuthOptions
{
    public TimeSpan AuthorizationExpiry { get; set; }
    public TimeSpan TokenExpiry { get; set; }
    public TimeSpan? RefreshTokenExpiry { get; set; }
}