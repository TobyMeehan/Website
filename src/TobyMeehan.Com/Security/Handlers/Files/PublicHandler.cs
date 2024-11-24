using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using TobyMeehan.Com.Domain.Downloads;

namespace TobyMeehan.Com.Security.Handlers.Files;

public class PublicHandler : AuthorizationHandler<OperationAuthorizationRequirement, DownloadFile>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        OperationAuthorizationRequirement requirement,
        DownloadFile resource)
    {
        if (requirement.Name != Requirements.File.View.Name)
        {
            return Task.CompletedTask;
        }

        if (resource.Visibility is not (Visibility.Public or Visibility.Unlisted))
        {
            return Task.CompletedTask;
        }
        
        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}