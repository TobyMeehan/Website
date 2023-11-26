using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace TobyMeehan.Com.Api.Security.Download.Handlers;

public class SameAuthorCreateHandler : AuthorizationHandler<OperationAuthorizationRequirement, Resource<IDownload>>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement,
        Resource<IDownload> resource)
    {
        if (requirement.Name != OperationRequirements.Create.Name)
        {
            return;
        }

        if (context.User.GetSubject() == resource.User.Id.Value)
        {
            context.Succeed(requirement);
        }
    }
}