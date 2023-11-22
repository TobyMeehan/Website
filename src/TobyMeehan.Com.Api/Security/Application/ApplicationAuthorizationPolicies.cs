using Microsoft.AspNetCore.Authorization;

namespace TobyMeehan.Com.Api.Security.Application;

public class ApplicationAuthorizationPolicies
{
    public static AuthorizationPolicy Create { get; } = new AuthorizationPolicyBuilder()
        .RequireScope(ScopeNames.Applications.Create)
        .Build();

    public static AuthorizationPolicy Read { get; } = new AuthorizationPolicyBuilder()
        .RequireScope(ScopeNames.Applications.Read)
        .Build();

    public static AuthorizationPolicy Update { get; } = new AuthorizationPolicyBuilder()
        .RequireScope(ScopeNames.Applications.Update)
        .Build();

    public static AuthorizationPolicy Delete { get; } = new AuthorizationPolicyBuilder()
        .RequireScope(ScopeNames.Applications.Delete)
        .Build();
}