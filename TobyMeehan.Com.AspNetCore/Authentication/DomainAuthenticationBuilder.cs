using AspNet.Security.OAuth.Discord;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TobyMeehan.Com.AspNetCore.Authentication
{
    public class DomainAuthenticationBuilder
    {
        public DomainAuthenticationBuilder(IServiceCollection services, string keyRingBucket, string dataProtectionObject, Action<AuthenticationOptions> configureOptions = null)
        {
            Services = services;

            services.AddDataProtection()
                .PersistKeysToGoogleCloudStorage(keyRingBucket, dataProtectionObject)
                .SetApplicationName("App.TobyMeehan.Com");

            AuthenticationBuilder = services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;

                configureOptions?.Invoke(options);
            });
        }

        // cookie authentication for tobymeehan.com
        public DomainAuthenticationBuilder AddLocalCookie(Action<CookieAuthenticationOptions> configureOptions = null)
        {
            AuthenticationBuilder.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.LoginPath = "/login";
                options.AccessDeniedPath = "/login";
                options.LogoutPath = "/logout";

                options.Cookie.Name = "TobyMeehan.Com.Authentication";

#if DEBUG
                options.Cookie.Domain = "localhost";
#else
                options.Cookie.Domain = ".tobymeehan.com";
#endif

                options.ExpireTimeSpan = TimeSpan.FromDays(7);
                options.SlidingExpiration = true;

                configureOptions?.Invoke(options);
            });

            return this;
        }

        // cookie authentication for subdomains of tobymeehan.com
        public DomainAuthenticationBuilder AddRemoteCookie(Action<CookieAuthenticationOptions> configureOptions = null)
        {
            Services.AddScoped<RemoteCookieAuthenticationEvents>();

            return AddLocalCookie(options =>
            {
                options.EventsType = typeof(RemoteCookieAuthenticationEvents);
            });
        }

        public DomainAuthenticationBuilder AddDiscord(Action<DiscordAuthenticationOptions> configureOptions = null)
        {
            Services.AddScoped<DiscordOAuthEvents>();

            AuthenticationBuilder.AddDiscord("Discord", options =>
            {
                options.Scope.Add("identify");

                options.CallbackPath = "/login/discord/callback";
                options.SaveTokens = true;

                options.EventsType = typeof(DiscordOAuthEvents);

                configureOptions?.Invoke(options);
            });

            return this;
        }

        public IServiceCollection Services { get; }

        public AuthenticationBuilder AuthenticationBuilder { get; }
    }
}
