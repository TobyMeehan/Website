using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace TobyMeehan.Com.Accounts.Authentication;

public static class AuthenticationBuilderExtensions
{
    public static AuthenticationBuilder AddCookieAuthentication(this AuthenticationBuilder builder)
    {
        builder.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
        {
            options.LoginPath = "/login";
            options.AccessDeniedPath = "/login";

            options.ExpireTimeSpan = DateTimeOffset.UtcNow.AddMonths(6) - DateTimeOffset.UtcNow;
            options.SlidingExpiration = true;
        });

        builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

        return builder;
    }
}