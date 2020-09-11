using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TobyMeehan.Com.AspNetCore.Authentication
{
    public class SharedCookieAuthenticationBuilder
    {
        public SharedCookieAuthenticationBuilder(IServiceCollection services, string keyRingBucket, string dataProtectionObject, Action<CookieAuthenticationOptions> configureOptions = null)
        {
            Services = services;

            services.AddDataProtection()
                .PersistKeysToGoogleCloudStorage(keyRingBucket, dataProtectionObject)
                .SetApplicationName("App.TobyMeehan.Com");

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/login";
                    options.AccessDeniedPath = "/login";

                    options.Cookie.Name = "TobyMeehan.Com.Authentication";

#if DEBUG
                    options.Cookie.Domain = "localhost";
#else
                    options.Cookie.Domain = ".tobymeehan.com";
#endif

                    options.ExpireTimeSpan = DateTimeOffset.UtcNow.AddMonths(6).Subtract(DateTimeOffset.UtcNow);
                    options.SlidingExpiration = true;

                    configureOptions?.Invoke(options);
                });
        }

        public IServiceCollection Services { get; }
    }
}
