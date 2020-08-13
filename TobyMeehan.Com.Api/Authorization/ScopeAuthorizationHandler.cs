using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Api.Authorization
{
    public class ScopeAuthorizationHandler : AuthorizationHandler<ScopeAuthorizationRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ScopeAuthorizationRequirement requirement)
        {
            if (requirement.Scopes.All(scope => context.User.HasClaim(Scopes.ClaimType, scope)))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

    public class ScopeAuthorizationRequirement : IAuthorizationRequirement
    {
        public ScopeAuthorizationRequirement(params string[] scopes)
        {
            Scopes = scopes;
        }

        public IEnumerable<string> Scopes { get; set; }
    }
}
