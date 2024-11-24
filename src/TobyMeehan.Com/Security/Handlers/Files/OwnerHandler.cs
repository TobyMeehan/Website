using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using TobyMeehan.Com.Domain.Downloads;
using TobyMeehan.Com.Domain.Downloads.Services;

namespace TobyMeehan.Com.Security.Handlers.Files;

public class OwnerHandler : AuthorizationHandler<OperationAuthorizationRequirement, DownloadFile>
{
    private readonly IDownloadAuthorService _downloadAuthorService;

    public OwnerHandler(IDownloadAuthorService downloadAuthorService)
    {
        _downloadAuthorService = downloadAuthorService;
    }
    
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        OperationAuthorizationRequirement requirement,
        DownloadFile resource)
    {
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId is null)
        {
            return;
        }
        
        var isOwner = await _downloadAuthorService.IsOwnerAsync(
            downloadId: resource.DownloadId,
            userId: new Guid(userId));
        
        if (isOwner)
        {
            context.Succeed(requirement);
        }
    }
}