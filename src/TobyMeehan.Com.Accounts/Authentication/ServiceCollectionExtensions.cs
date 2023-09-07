using Microsoft.AspNetCore.Authentication.Cookies;

namespace TobyMeehan.Com.Accounts.Authentication;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCookieAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/login";
                options.AccessDeniedPath = "/login";

                options.ExpireTimeSpan = DateTimeOffset.UtcNow.AddMonths(6) - DateTimeOffset.UtcNow;
                options.SlidingExpiration = true;
            });

        services.AddScoped<IAuthenticationService, AuthenticationService>();

        return services;
    }
}