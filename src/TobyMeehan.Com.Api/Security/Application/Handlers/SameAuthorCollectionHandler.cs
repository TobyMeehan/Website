using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using TobyMeehan.Com.Api.CollectionAuthorization;

namespace TobyMeehan.Com.Api.Security.Application.Handlers;

public class SameAuthorCollectionHandler : CollectionAuthorizationHandler<OperationAuthorizationRequirement, IApplication, IUser>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement,
        IUser collector)
    {
        if (requirement.Name == OperationRequirements.Read.Name && 
            context.User.GetSubject() == collector.Id.Value)
        {
            context.Succeed(requirement);
        }
        
        return Task.CompletedTask;
    }
}