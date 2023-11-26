using Microsoft.AspNetCore.Authorization;

namespace TobyMeehan.Com.Api.Security.Download;

public class DownloadScopePolicies
{
    public static readonly AuthorizationPolicy Create = new AuthorizationPolicyBuilder()
        .RequireScope(ScopeNames.Downloads.Create)
        .Build();

    public static readonly AuthorizationPolicy Read = new AuthorizationPolicyBuilder()
        .RequireScope(ScopeNames.Downloads.Read)
        .Build();

    public static readonly AuthorizationPolicy Update = new AuthorizationPolicyBuilder()
        .RequireScope(ScopeNames.Downloads.Update)
        .Build();

    public static readonly AuthorizationPolicy Delete = new AuthorizationPolicyBuilder()
        .RequireScope(ScopeNames.Downloads.Delete)
        .Build();
}