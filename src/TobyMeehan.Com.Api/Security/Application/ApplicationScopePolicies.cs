using Microsoft.AspNetCore.Authorization;

namespace TobyMeehan.Com.Api.Security.Application;

public class ApplicationScopePolicies
{
    public static readonly AuthorizationPolicy Create = new AuthorizationPolicyBuilder()
        .RequireScope(ScopeNames.Applications.Create)
        .Build();

    public static readonly AuthorizationPolicy Read = new AuthorizationPolicyBuilder()
        .RequireScope(ScopeNames.Applications.Read)
        .Build();

    public static readonly AuthorizationPolicy Update = new AuthorizationPolicyBuilder()
        .RequireScope(ScopeNames.Applications.Update)
        .Build();

    public static readonly AuthorizationPolicy Delete = new AuthorizationPolicyBuilder()
        .RequireScope(ScopeNames.Applications.Delete)
        .Build();
}