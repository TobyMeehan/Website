using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace TobyMeehan.Com.Api.Security.User;

public class UserOperationAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, IUser>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement,
        IUser resource)
    {
        if (requirement.Name == OperationRequirements.Read.Name)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        if (requirement.Name == OperationRequirements.Update.Name ||
            requirement.Name == OperationRequirements.Delete.Name &&
            context.User.GetSubject() == resource.Id)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}