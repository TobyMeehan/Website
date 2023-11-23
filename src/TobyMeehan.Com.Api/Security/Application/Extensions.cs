using Microsoft.AspNetCore.Authorization;
using TobyMeehan.Com.Api.Security.Application.Handlers;

namespace TobyMeehan.Com.Api.Security.Application;

public static class Extensions
{
    public static AuthorizationBuilder AddApplicationPolicies(this AuthorizationBuilder builder) => builder
            
        .AddPolicy(PolicyNames.Application.Scope.Create, ApplicationScopePolicies.Create)
        .AddPolicy(PolicyNames.Application.Scope.Read, ApplicationScopePolicies.Read)
        .AddPolicy(PolicyNames.Application.Scope.Update, ApplicationScopePolicies.Update)
        .AddPolicy(PolicyNames.Application.Scope.Delete, ApplicationScopePolicies.Delete)

        .AddPolicy(PolicyNames.Application.Operation.Create, ApplicationOperationPolicies.Create)
        .AddPolicy(PolicyNames.Application.Operation.Read, ApplicationOperationPolicies.Read)
        .AddPolicy(PolicyNames.Application.Operation.Update, ApplicationOperationPolicies.Update)
        .AddPolicy(PolicyNames.Application.Operation.Delete, ApplicationOperationPolicies.Delete)
    
    ;

    public static IServiceCollection AddApplicationAuthorizationHandlers(this IServiceCollection services) => services
            
        .AddSingleton<IAuthorizationHandler, SameAuthorCreateHandler>()
        .AddSingleton<IAuthorizationHandler, ReadHandler>()
        .AddSingleton<IAuthorizationHandler, SameAuthorCollectionHandler>()
        .AddSingleton<IAuthorizationHandler, SameAuthorUpdateDeleteHandler>()
    
    ;
}