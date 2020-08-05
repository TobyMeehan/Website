using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Extensions;

namespace TobyMeehan.Com.Authorization
{
    public class CanEditApplicationAuthorizationHandler : AuthorizationHandler<EditApplicationRequirement, Application>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, EditApplicationRequirement requirement, Application resource)
        {
            if (resource.Author.Id == context.User.Id())
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

    public class EditApplicationRequirement : IAuthorizationRequirement { }
}
