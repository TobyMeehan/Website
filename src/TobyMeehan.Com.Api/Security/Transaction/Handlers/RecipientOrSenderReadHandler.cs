using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace TobyMeehan.Com.Api.Security.Transaction.Handlers;

public class RecipientOrSenderReadHandler : AuthorizationHandler<OperationAuthorizationRequirement, ITransaction>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement,
        ITransaction resource)
    {
        if (context.User.GetSubject() == resource.RecipientId ||
            context.User.GetSubject() == resource.SenderId)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}