using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using TobyMeehan.Com.AspNetCore.Authentication;
using TobyMeehan.Com.AspNetCore.Authorization;

namespace TobyMeehan.Com.AspNetCore
{
    public static class ServiceCollectionExtensions
    {
        public static CustomAuthorizationBuilder AddCustomAuthorization(this IServiceCollection services)
        {
            return new CustomAuthorizationBuilder(services);
        }

        public static SharedCookieAuthenticationBuilder AddSharedCookieAuthentication(this IServiceCollection services, string keyRingPath, Action<CookieAuthenticationOptions> configureOptions = null)
        {
            return new SharedCookieAuthenticationBuilder(services, keyRingPath, configureOptions);
        }
    }
}
