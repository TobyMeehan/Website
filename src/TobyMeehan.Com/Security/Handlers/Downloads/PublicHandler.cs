using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using TobyMeehan.Com.Domain.Downloads;

namespace TobyMeehan.Com.Security.Handlers.Downloads;

public class PublicHandler : AuthorizationHandler<OperationAuthorizationRequirement, Download>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        OperationAuthorizationRequirement requirement,
        Download resource)
    {
        if (requirement.Name != Requirements.Download.View.Name)
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