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
        private readonly string _keyRingPath;

        public SharedCookieAuthenticationBuilder(IServiceCollection services, string keyRingPath, Action<CookieAuthenticationOptions> configureOptions = null)
        {
            Services = services;
            _keyRingPath = keyRingPath;

            services.AddDataProtection()
                .PersistKeysToFileSystem(GetKeyRingDirectoryInfo(keyRingPath))
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

        private DirectoryInfo GetKeyRingDirectoryInfo(string keyRingPath)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(Directory.GetCurrentDirectory());

            do
            {
                directoryInfo = directoryInfo.Parent;

                DirectoryInfo keyRingDirectoryInfo = new DirectoryInfo(Path.Combine(directoryInfo.FullName, keyRingPath));

                if (keyRingDirectoryInfo.Exists)
                {
                    return keyRingDirectoryInfo;
                }
            }
            while (directoryInfo.Parent != null);

            throw new Exception("Key ring path not found.");
        }
    }
}
