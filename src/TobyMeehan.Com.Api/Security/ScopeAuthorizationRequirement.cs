using Microsoft.AspNetCore.Authorization;

namespace TobyMeehan.Com.Api.Security;

public class ScopeAuthorizationRequirement : IAuthorizationRequirement
{
    public ScopeAuthorizationRequirement(string scope)
    {
        Scope = scope;
    }

    public string Scope { get; }
}