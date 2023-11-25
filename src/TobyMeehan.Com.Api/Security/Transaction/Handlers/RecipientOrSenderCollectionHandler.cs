using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using TobyMeehan.Com.Api.CollectionAuthorization;

namespace TobyMeehan.Com.Api.Security.Transaction.Handlers;

public class RecipientOrSenderCollectionHandler : CollectionAuthorizationHandler<OperationAuthorizationRequirement, ITransaction, IUser>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement,
        IUser collector)
    {
        if (context.User.GetSubject() == collector.Id.Value)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}