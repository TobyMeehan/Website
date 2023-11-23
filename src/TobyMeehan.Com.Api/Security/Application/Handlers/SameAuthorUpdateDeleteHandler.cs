using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace TobyMeehan.Com.Api.Security.Application.Handlers;

public class SameAuthorUpdateDeleteHandler : AuthorizationHandler<OperationAuthorizationRequirement, IApplication>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement,
        IApplication resource)
    {
        if (requirement.Name != OperationRequirements.Update.Name &&
            requirement.Name != OperationRequirements.Delete.Name)
        {
            return Task.CompletedTask;
        }
        
        if (context.User.GetSubject() == resource.AuthorId)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}