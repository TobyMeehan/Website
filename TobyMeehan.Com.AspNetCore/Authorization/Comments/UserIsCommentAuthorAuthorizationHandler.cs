using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TobyMeehan.Com.AspNetCore.Extensions;
using TobyMeehan.Com.Data.Models;

namespace TobyMeehan.Com.AspNetCore.Authorization.Comments
{
    public class UserIsCommentAuthorAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Comment>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, Comment resource)
        {
            if (context.User == null || resource == null)
            {
                return Task.CompletedTask;
            }

            if (!requirement.IsCrudOperationRequirement())
            {
                return Task.CompletedTask;
            }

            if (resource.User.Id == context.User.Id())
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
