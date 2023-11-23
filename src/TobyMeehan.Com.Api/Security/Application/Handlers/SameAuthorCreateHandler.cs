using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace TobyMeehan.Com.Api.Security.Application.Handlers;

public class SameAuthorCreateHandler : AuthorizationHandler<OperationAuthorizationRequirement, IUser>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement,
        IUser resource)
    {
        if (requirement.Name != OperationRequirements.Create.Name)
        {
            return Task.CompletedTask;
        }

        if (context.User.GetSubject() == resource.Id)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}