using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Extensions;

namespace TobyMeehan.Com.Authorization
{
    public class CanEditDownloadAuthorizationHandler : AuthorizationHandler<EditDownloadRequirement, Download>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, EditDownloadRequirement requirement, Download resource)
        {
            if (resource.Authors.Any(u => u.Id == context.User.Id()))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

    public class EditDownloadRequirement : IAuthorizationRequirement { }
}
