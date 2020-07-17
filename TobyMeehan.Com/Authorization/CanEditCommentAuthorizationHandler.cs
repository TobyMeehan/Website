using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TobyMeehan.Com.Data;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Extensions;

namespace TobyMeehan.Com.Authorization
{
    public class CanEditCommentAuthorizationHandler : AuthorizationHandler<EditCommentRequirement, Comment>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, EditCommentRequirement requirement, Comment resource)
        {
            if (resource.User.Id == context.User.Id() || context.User.IsInRole(UserRoles.Admin))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

    public class EditCommentRequirement : IAuthorizationRequirement { }
}
