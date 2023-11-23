using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace TobyMeehan.Com.Api.Security.User.Handlers;

public class OperationHandler : AuthorizationHandler<OperationAuthorizationRequirement, IUser>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement,
        IUser resource)
    {
        if (requirement.Name == OperationRequirements.Read.Name)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
        
        if (context.User.GetSubject() == resource.Id)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}