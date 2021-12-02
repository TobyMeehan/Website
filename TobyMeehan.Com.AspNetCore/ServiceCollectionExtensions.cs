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

        public static DomainAuthenticationBuilder AddDomainAuthentication(this IServiceCollection services, string keyRingBucket, string dataProtectionObject, Action<AuthenticationOptions> configureOptions = null)
        {
            return new DomainAuthenticationBuilder(services, keyRingBucket, dataProtectionObject, configureOptions);
        }
    }
}
