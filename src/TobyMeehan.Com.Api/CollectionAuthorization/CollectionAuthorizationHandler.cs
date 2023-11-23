using Microsoft.AspNetCore.Authorization;

namespace TobyMeehan.Com.Api.CollectionAuthorization;

public abstract class CollectionAuthorizationHandler<TRequirement, TResource, TCollector> :
    AuthorizationHandler<TRequirement, ResourceCollectionDescriptor<TResource, TCollector>> 
    where TRequirement : IAuthorizationRequirement
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, TRequirement requirement,
        ResourceCollectionDescriptor<TResource, TCollector> collection)
    {
        await HandleRequirementAsync(context, requirement, collection.Collector);
    }

    protected abstract Task HandleRequirementAsync(
        AuthorizationHandlerContext context, TRequirement requirement, TCollector collector);
}