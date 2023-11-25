using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace TobyMeehan.Com.Api.Security.Application.Handlers;

public class SameAuthorCreateHandler : AuthorizationHandler<OperationAuthorizationRequirement, Resource<IApplication>>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement,
        Resource<IApplication> resource)
    {
        if (requirement.Name != OperationRequirements.Create.Name)
        {
            return Task.CompletedTask;
        }

        if (context.User.GetSubject() == resource.User.Id.Value)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}