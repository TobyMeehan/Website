using Microsoft.AspNetCore.Authorization;
using TobyMeehan.Com.Api.Security.Application;
using TobyMeehan.Com.Api.Security.User;

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
            
            options.AddPolicy(ScopeNames.Account.Update, UserAuthorizationPolicies.Update);
            options.AddPolicy(ScopeNames.Account.Password, UserAuthorizationPolicies.Password);
            options.AddPolicy(ScopeNames.Account.Delete, UserAuthorizationPolicies.Delete);
        })
        
        .AddSingleton<IAuthorizationHandler, ScopeAuthorizationHandler>()
        
        .AddSingleton<IAuthorizationHandler, ApplicationAuthorAuthorizationHandler>()
        .AddSingleton<IAuthorizationHandler, ApplicationReadAuthorizationHandler>()
    
        .AddSingleton<IAuthorizationHandler, UserOperationAuthorizationHandler>()
    
    ;
}