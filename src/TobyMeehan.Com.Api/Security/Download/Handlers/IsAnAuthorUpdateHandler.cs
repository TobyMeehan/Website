using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace TobyMeehan.Com.Api.Security.Download.Handlers;

public class IsAnAuthorUpdateHandler : AuthorizationHandler<OperationAuthorizationRequirement, IDownload>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement,
        IDownload resource)
    {
        if (requirement.Name != OperationRequirements.Update.Name)
        {
            return;
        }

        if (context.User.GetSubject() is not { } subject)
        {
            return;
        }

        var author = resource.Authors.Find(subject);

        if (author is { CanEdit: true })
        {
            context.Succeed(requirement);
        }
    }
}