using Microsoft.AspNetCore.Authorization;
using TobyMeehan.Com.Api.Security.Download.Handlers;

namespace TobyMeehan.Com.Api.Security.Download;

public static class Extensions
{
    public static AuthorizationBuilder AddDownloadPolicies(this AuthorizationBuilder builder) => builder

        .AddPolicy(PolicyNames.Download.Scope.Create, DownloadScopePolicies.Create)
        .AddPolicy(PolicyNames.Download.Scope.Read, DownloadScopePolicies.Read)
        .AddPolicy(PolicyNames.Download.Scope.Update, DownloadScopePolicies.Update)
        .AddPolicy(PolicyNames.Download.Scope.Delete, DownloadScopePolicies.Delete)

        .AddPolicy(PolicyNames.Download.Operation.Create, DownloadOperationPolicies.Create)
        .AddPolicy(PolicyNames.Download.Operation.Read, DownloadOperationPolicies.Read)
        .AddPolicy(PolicyNames.Download.Operation.Update, DownloadOperationPolicies.Update)
        .AddPolicy(PolicyNames.Download.Operation.Delete, DownloadOperationPolicies.Delete)
    
    ;

    public static IServiceCollection AddDownloadAuthorizationHandlers(this IServiceCollection services) => services

        .AddSingleton<IAuthorizationHandler, CollectionReadHandler>()
        .AddSingleton<IAuthorizationHandler, IsAnAuthorDeleteHandler>()
        .AddSingleton<IAuthorizationHandler, IsAnAuthorUpdateHandler>()
        .AddSingleton<IAuthorizationHandler, IsAnAuthorReadHandler>()
        .AddSingleton<IAuthorizationHandler, PublicReadHandler>()
        .AddSingleton<IAuthorizationHandler, SameAuthorCreateHandler>()
    
    ;
}