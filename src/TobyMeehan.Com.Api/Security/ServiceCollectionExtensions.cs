using Microsoft.AspNetCore.Authorization;
using TobyMeehan.Com.Api.Security.Application;
using TobyMeehan.Com.Api.Security.User;

namespace TobyMeehan.Com.Api.Security;

public static class ServiceCollectionExtensions
{
    public static AuthorizationBuilder AddSecurityPolicies(this IServiceCollection services) => services
        .AddSingleton<IAuthorizationHandler, ScopeAuthorizationHandler>()
            
        .AddApplicationAuthorizationHandlers()
        .AddUserAuthorizationHandlers()
            
        .AddAuthorizationBuilder()
        .AddApplicationPolicies()
        .AddUserPolicies()
    
    ;
}