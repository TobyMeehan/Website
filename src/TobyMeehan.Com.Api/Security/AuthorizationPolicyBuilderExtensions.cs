using Microsoft.AspNetCore.Authorization;

namespace TobyMeehan.Com.Api.Security;

public static class AuthorizationPolicyBuilderExtensions
{
    public static AuthorizationPolicyBuilder RequireScope(this AuthorizationPolicyBuilder builder, string scope)
    {
        builder.AddRequirements(new ScopeAuthorizationRequirement(scope));
        
        return builder;
    }
}