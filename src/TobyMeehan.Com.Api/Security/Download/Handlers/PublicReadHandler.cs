using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace TobyMeehan.Com.Api.Security.Download.Handlers;

public class PublicReadHandler : AuthorizationHandler<OperationAuthorizationRequirement, IDownload>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement,
        IDownload resource)
    {
        if (requirement.Name != OperationRequirements.Read.Name)
        {
            return;
        }
        
        if (resource.Visibility is VisibilityNames.Public or VisibilityNames.Unlisted)
        {
            context.Succeed(requirement);
        }
    }
}