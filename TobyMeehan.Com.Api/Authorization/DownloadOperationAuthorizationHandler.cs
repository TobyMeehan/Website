using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TobyMeehan.Com.Api.Extensions;
using TobyMeehan.Com.Api.Models;

namespace TobyMeehan.Com.Api.Authorization
{
    public class DownloadOperationAuthorizationHandler : AuthorizationHandler<AuthorizationRequirement, DownloadModel>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizationRequirement requirement, DownloadModel resource)
        {
            switch (requirement.Operation)
            {
                case Operation.Create: // User must be verified, session must have downloads scope

                    if (!context.User.IsInRole(Roles.Scopes.Downloads))
                    {
                        requirement.FailureMessage = "The 'downloads' scope is required to create a download.";
                        break;
                    }

                    if (!context.User.IsInRole(Roles.User.Verified))
                    {
                        requirement.FailureMessage = "User must be verified to create a download.";
                        break;
                    }

                    context.Succeed(requirement);
                    break;
                case Operation.Read: // No conditions

                    context.Succeed(requirement);

                    break;
                case Operation.Update: // User must be an author of the download, session must have downloads scope

                    if (!context.User.IsInRole(Roles.Scopes.Downloads))
                    {
                        requirement.FailureMessage = "The 'downloads' scope is required to update a download.";
                        break;
                    }

                    if (!resource?.Authors?.Any(u => u.Id == context.User.Id()) ?? false)
                    {
                        requirement.FailureMessage = "User must be an author to update a download.";
                        break;
                    }

                    context.Succeed(requirement);

                    break;
                case Operation.Delete: // User must be an author of the download, session must have downloads scope

                    if (!context.User.IsInRole(Roles.Scopes.Downloads))
                    {
                        requirement.FailureMessage = "The 'downloads' scope is required to delete a download.";
                        break;
                    }

                    if (!resource?.Authors?.Any(u => u.Id == context.User.Id()) ?? false)
                    {
                        requirement.FailureMessage = "User must be an author to delete a download.";
                        break;
                    }

                    context.Succeed(requirement);

                    break;
            }

            return Task.CompletedTask;
        }
    }
}
