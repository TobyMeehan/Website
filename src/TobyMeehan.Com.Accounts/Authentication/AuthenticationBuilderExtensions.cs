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

    public static AuthenticationBuilder AddClientBasicAuthentication(this AuthenticationBuilder builder, Action<AuthenticationSchemeOptions>? options = null)
    {
        builder.AddScheme<AuthenticationSchemeOptions, ClientBasicAuthenticationHandler>(
                ClientBasicAuthenticationHandler.AuthenticationScheme, options);

        return builder;
    }
}