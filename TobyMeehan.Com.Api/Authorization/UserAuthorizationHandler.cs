using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TobyMeehan.Com.Api.Extensions;
using TobyMeehan.Com.Api.Models;

namespace TobyMeehan.Com.Api.Authorization
{
    public class UserAuthorizationHandler : AuthorizationHandler<IdentifyScopeRequirement, UserModel>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IdentifyScopeRequirement requirement, UserModel resource)
        {
            if (context.User.IsInRole(Roles.Scopes.Identify) && resource.Id == context.User.Id())
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

    public class IdentifyScopeRequirement : IAuthorizationRequirement { }
}
