using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace TobyMeehan.Com.Api.Security.Transaction.Handlers;

public class SameUserCreateHandler : AuthorizationHandler<OperationAuthorizationRequirement, Resource<ITransaction>>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement,
        Resource<ITransaction> resource)
    {
        if (requirement.Name != OperationRequirements.Create.Name && requirement.Name != "Transfer")
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