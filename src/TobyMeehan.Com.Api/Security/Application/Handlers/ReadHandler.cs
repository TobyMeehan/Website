using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace TobyMeehan.Com.Api.Security.Application.Handlers;

public class ReadHandler : AuthorizationHandler<OperationAuthorizationRequirement, IApplication>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement,
        IApplication resource)
    {
        if (requirement.Name == OperationRequirements.Read.Name)
        {
            context.Succeed(requirement);
        }
        
        return Task.CompletedTask;
    }
}