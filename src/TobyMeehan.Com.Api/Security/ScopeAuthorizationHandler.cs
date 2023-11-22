using Microsoft.AspNetCore.Authorization;
using OpenIddict.Abstractions;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Api.Security;

public class ScopeAuthorizationHandler : AuthorizationHandler<ScopeAuthorizationRequirement>
{
    private readonly IScopeService _scopes;

    public ScopeAuthorizationHandler(IScopeService scopes)
    {
        _scopes = scopes;
    }
    
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ScopeAuthorizationRequirement requirement)
    {
        var result = await _scopes.GetByNameAsync(requirement.Scope);

        if (!result.IsSuccess(out var scope))
        {
            return;
        }
        
        string[] subScopes = scope.Name.Split('.');

        for (int i = 1; i <= subScopes.Length; i++)
        {
            string combined = string.Join('.', subScopes.Take(i));

            if (context.User.HasScope(combined))
            {
                context.Succeed(requirement);
            }
        }
    }
}
