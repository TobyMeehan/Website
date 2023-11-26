using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using TobyMeehan.Com.Api.CollectionAuthorization;

namespace TobyMeehan.Com.Api.Security.Download.Handlers;

public class CollectionReadHandler : CollectionAuthorizationHandler<OperationAuthorizationRequirement, IDownload, IUser>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement,
        IUser collector)
    {
        if (requirement.Name != OperationRequirements.Read.Name)
        {
            return;
        }
        
        context.Succeed(requirement);
    }
}