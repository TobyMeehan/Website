using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using TobyMeehan.Com.AspNetCore.Authorization;

namespace TobyMeehan.Com.AspNetCore
{
    public static class ServiceCollectionExtensions
    {
        public static CustomAuthorizationBuilder AddCustomAuthorization(this IServiceCollection services)
        {
            return new CustomAuthorizationBuilder(services);
        }
    }
}
