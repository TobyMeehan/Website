using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorUI.Authorization
{
    public class VerifiedAccountAuthorizationHandler : AuthorizationHandler<VerifiedAccountRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, VerifiedAccountRequirement requirement)
        {
            if (context.User.IsInRole("Verified") || context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

    public class VerifiedAccountRequirement : IAuthorizationRequirement { }
}
