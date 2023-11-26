using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace TobyMeehan.Com.Api.Security.Download.Handlers;

public class IsAnAuthorDeleteHandler : AuthorizationHandler<OperationAuthorizationRequirement, IDownload>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement,
        IDownload resource)
    {
        if (requirement.Name != OperationRequirements.Delete.Name)
        {
            return;
        }

        if (context.User.GetSubject() is not { } subject)
        {
            return;
        }

        var author = resource.Authors.Find(subject);

        if (author is { CanDelete: true })
        {
            context.Succeed(requirement);
        }
    }
}