using Microsoft.AspNetCore.Authorization;
using TobyMeehan.Com.Api.Security.Application;

namespace TobyMeehan.Com.Api.Security;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSecurityPolicies(this IServiceCollection services) => services
        .AddAuthorization(options =>
        {
            options.AddPolicy(ScopeNames.Applications.Create, ApplicationAuthorizationPolicies.Create);
            options.AddPolicy(ScopeNames.Applications.Read, ApplicationAuthorizationPolicies.Read);
            options.AddPolicy(ScopeNames.Applications.Update, ApplicationAuthorizationPolicies.Update);
            options.AddPolicy(ScopeNames.Applications.Delete, ApplicationAuthorizationPolicies.Delete);
        })
        
        .AddSingleton<IAuthorizationHandler, ScopeAuthorizationHandler>()
        
        .AddSingleton<IAuthorizationHandler, ApplicationAuthorAuthorizationHandler>()
        .AddSingleton<IAuthorizationHandler, ApplicationReadAuthorizationHandler>()
    
    ;
}