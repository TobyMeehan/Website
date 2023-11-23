using Microsoft.AspNetCore.Authorization;
using TobyMeehan.Com.Api.Security.User.Handlers;

namespace TobyMeehan.Com.Api.Security.User;

public static class Extensions
{
    public static AuthorizationBuilder AddUserPolicies(this AuthorizationBuilder builder) => builder

        .AddPolicy(PolicyNames.User.Scope.Identify, UserScopePolicies.Identify)
        .AddPolicy(PolicyNames.User.Scope.Update, UserScopePolicies.Update)
        .AddPolicy(PolicyNames.User.Scope.Password, UserScopePolicies.Password)
        .AddPolicy(PolicyNames.User.Scope.Delete, UserScopePolicies.Delete)

        .AddPolicy(PolicyNames.User.Operation.Read, UserOperationPolicies.Read)
        .AddPolicy(PolicyNames.User.Operation.Identify, UserOperationPolicies.Identify)
        .AddPolicy(PolicyNames.User.Operation.Update, UserOperationPolicies.Update)
        .AddPolicy(PolicyNames.User.Operation.Password, UserOperationPolicies.Password)
        .AddPolicy(PolicyNames.User.Operation.Delete, UserOperationPolicies.Delete)
    
    ;

    public static IServiceCollection AddUserAuthorizationHandlers(this IServiceCollection services) => services

        .AddSingleton<IAuthorizationHandler, OperationHandler>()
    
    ;
}