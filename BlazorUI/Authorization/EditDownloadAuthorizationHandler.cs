using BlazorUI.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorUI.Authorization
{
    public class EditDownloadAuthorizationHandler : AuthorizationHandler<DownloadAuthorRequirement, Download>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DownloadAuthorRequirement requirement, Download resource)
        {
            if (resource.Authors.Any(x => x.Id == context.User.Identity.Name) || context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

    public class DownloadAuthorRequirement : IAuthorizationRequirement { }
}
