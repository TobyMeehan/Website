using Microsoft.AspNetCore.Authorization;

namespace TobyMeehan.Com.Api.Security.User;

public class UserAuthorizationPolicies
{
    public static readonly AuthorizationPolicy Update = new AuthorizationPolicyBuilder()
        .RequireScope(ScopeNames.Account.Update)
        .Build();

    public static readonly AuthorizationPolicy Password = new AuthorizationPolicyBuilder()
        .RequireScope(ScopeNames.Account.Password)
        .Build();

    public static readonly AuthorizationPolicy Delete = new AuthorizationPolicyBuilder()
        .RequireScope(ScopeNames.Account.Delete)
        .Build();
}