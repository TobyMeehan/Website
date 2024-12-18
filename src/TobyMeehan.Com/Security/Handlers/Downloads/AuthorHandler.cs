using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using TobyMeehan.Com.Domain.Downloads;
using TobyMeehan.Com.Domain.Downloads.Services;

namespace TobyMeehan.Com.Security.Handlers.Downloads;

public class AuthorHandler : AuthorizationHandler<OperationAuthorizationRequirement, Download>
{
    private readonly IDownloadAuthorService _downloadAuthorService;

    public AuthorHandler(IDownloadAuthorService downloadAuthorService)
    {
        _downloadAuthorService = downloadAuthorService;
    }
    
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        OperationAuthorizationRequirement requirement,
        Download resource)
    {
        List<string> validRequirements =
        [
            Requirements.Download.View.Name,
            Requirements.Download.Manage.Name,
            Requirements.Download.Edit.Name,
            Requirements.Download.Delete.Name,
            Requirements.Download.Files.Upload.Name,
            Requirements.Download.Authors.Invite.Name,
            Requirements.Download.Authors.Kick.Name
        ];
        
        if (!validRequirements.Contains(requirement.Name))
        {
            return;
        }

        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId is null)
        {
            return;
        }

        var isAuthor = await _downloadAuthorService.IsAuthorAsync(
            downloadId: resource.Id,
            userId: new Guid(userId));

        if (!isAuthor)
        {
            return;
        }
        
        context.Succeed(requirement);
    }
}