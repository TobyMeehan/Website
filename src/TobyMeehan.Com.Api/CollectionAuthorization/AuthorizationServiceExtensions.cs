using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace TobyMeehan.Com.Api.CollectionAuthorization;

public static class AuthorizationServiceExtensions
{
    public static async Task<CollectionAuthorizationResult<TResource>>
        AuthorizeAsync<TResource>(
            this IAuthorizationService authorizationService,
            ClaimsPrincipal user,
            IEnumerable<TResource> resources,
            IEnumerable<IAuthorizationRequirement> requirements
        ) where TResource : notnull

        => await AuthorizeAsync(resources, 
            resource => authorizationService.AuthorizeAsync(user, resource, requirements));

    public static async Task<CollectionAuthorizationResult<TResource>>
        AuthorizeAsync<TResource>(
            this IAuthorizationService authorizationService,
            ClaimsPrincipal user,
            IEnumerable<TResource> resources,
            IAuthorizationRequirement requirement
        ) where TResource : notnull

        => await AuthorizeAsync(resources,
            resource => authorizationService.AuthorizeAsync(user, resource, requirement));

    public static async Task<CollectionAuthorizationResult<TResource>>
        AuthorizeAsync<TResource>(
            this IAuthorizationService authorizationService,
            ClaimsPrincipal user,
            IEnumerable<TResource> resources,
            string policyName
        ) where TResource : notnull

        => await AuthorizeAsync(resources,
            resource => authorizationService.AuthorizeAsync(user, resource, policyName));

    private static async Task<CollectionAuthorizationResult<TResource>>
        AuthorizeAsync<TResource>(
            IEnumerable<TResource> resources,
            AuthorizeDelegate authorizeDelegate) where TResource : notnull
    {
        var results = new List<(TResource, AuthorizationResult)>();

        foreach (var resource in resources)
        {
            var result = await authorizeDelegate(resource);
            
            results.Add((resource, result));
        }

        return new CollectionAuthorizationResult<TResource>(results);
    }
    
    public static async Task<CollectionAuthorizationResult<TResource, TCollector>>
        AuthorizeAsync<TResource, TCollector>(
            this IAuthorizationService authorizationService,
            ClaimsPrincipal user,
            IEnumerable<TResource> resources,
            TCollector collector,
            IEnumerable<IAuthorizationRequirement> requirements,
            bool authorizeResources = true
        ) where TResource : notnull

        => await AuthorizeAsync(GetDescriptor(resources, collector), authorizeResources,
            resource => authorizationService.AuthorizeAsync(user, resource, requirements));

    public static async Task<CollectionAuthorizationResult<TResource, TCollector>>
        AuthorizeAsync<TResource, TCollector>(
            this IAuthorizationService authorizationService,
            ClaimsPrincipal user,
            IEnumerable<TResource> resources,
            TCollector collector,
            IAuthorizationRequirement requirement,
            bool authorizeResources = true
        ) where TResource : notnull

        => await AuthorizeAsync(GetDescriptor(resources, collector), authorizeResources,
            resource => authorizationService.AuthorizeAsync(user, resource, requirement));

    public static async Task<CollectionAuthorizationResult<TResource, TCollector>>
        AuthorizeAsync<TResource, TCollector>(
            this IAuthorizationService authorizationService,
            ClaimsPrincipal user,
            IEnumerable<TResource> resources,
            TCollector collector,
            string policyName,
            bool authorizeResources = true
        ) where TResource : notnull

        => await AuthorizeAsync(GetDescriptor(resources, collector), authorizeResources,
            resource => authorizationService.AuthorizeAsync(user, resource, policyName));

    public static async Task<CollectionAuthorizationResult<TResource, TCollector>>
        AuthorizeAsync<TResource, TCollector>(
            this IAuthorizationService authorizationService,
            ClaimsPrincipal user,
            IEnumerable<TResource> resources,
            TCollector collector,
            AuthorizationPolicy policy,
            bool authorizeResources = true
        ) where TResource : notnull

        => await AuthorizeAsync(GetDescriptor(resources, collector), authorizeResources,
            resource => authorizationService.AuthorizeAsync(user, resource, policy));
    
    private static ResourceCollectionDescriptor<TResource, TCollector> GetDescriptor<TResource, TCollector>(
        IEnumerable<TResource> resources, TCollector collector)
    {
        return new ResourceCollectionDescriptor<TResource, TCollector>(resources.ToList(), collector);
    }
    
    public static async Task<CollectionAuthorizationResult<TResource, TCollector>>
        AuthorizeAsync<TResource, TCollector>(
            this IAuthorizationService authorizationService,
            ClaimsPrincipal user,
            IEnumerable<TResource> resources,
            Func<TResource, TCollector> selector,
            IEnumerable<IAuthorizationRequirement> requirements,
            bool authorizeResources = true
        ) where TResource : notnull 
    
        => await AuthorizeAsync(GetDescriptor(resources, selector), authorizeResources,
            resource => authorizationService.AuthorizeAsync(user, resource, requirements));

    public static async Task<CollectionAuthorizationResult<TResource, TCollector>>
        AuthorizeAsync<TResource, TCollector>(
            this IAuthorizationService authorizationService,
            ClaimsPrincipal user,
            IEnumerable<TResource> resources,
            Func<TResource, TCollector> selector,
            IAuthorizationRequirement requirement,
            bool authorizeResources = true
        ) where TResource : notnull
    
        => await AuthorizeAsync(GetDescriptor(resources, selector), authorizeResources,
            resource => authorizationService.AuthorizeAsync(user, resource, requirement));

    public static async Task<CollectionAuthorizationResult<TResource, TCollector>>
        AuthorizeAsync<TResource, TCollector>(
            this IAuthorizationService authorizationService,
            ClaimsPrincipal user,
            IEnumerable<TResource> resources,
            Func<TResource, TCollector> selector,
            string policyName,
            bool authorizeResources = true
        ) where TResource : notnull

        => await AuthorizeAsync(GetDescriptor(resources, selector), authorizeResources,
            resource => authorizationService.AuthorizeAsync(user, resource, policyName));
    
    public static async Task<CollectionAuthorizationResult<TResource, TCollector>>
        AuthorizeAsync<TResource, TCollector>(
            this IAuthorizationService authorizationService,
            ClaimsPrincipal user,
            IEnumerable<TResource> resources,
            Func<TResource, TCollector> selector,
            AuthorizationPolicy policy,
            bool authorizeResources = true
        ) where TResource : notnull

        => await AuthorizeAsync(GetDescriptor(resources, selector), authorizeResources,
            resource => authorizationService.AuthorizeAsync(user, resource, policy));

    private static ResourceCollectionDescriptor<TResource, TCollector> GetDescriptor<TResource, TCollector>(
        IEnumerable<TResource> resources, Func<TResource, TCollector> selector)
    {
        var collection = resources.ToList();

        var collector = collection.Select(selector).Distinct().Single();

        return new ResourceCollectionDescriptor<TResource, TCollector>(collection, collector);
    }
    
    private delegate Task<AuthorizationResult> AuthorizeDelegate(object? resource);
    
    private static async Task<CollectionAuthorizationResult<TResource, TCollector>> AuthorizeAsync<TResource, TCollector>(
        ResourceCollectionDescriptor<TResource, TCollector> descriptor,
        bool authorizeResources,
        AuthorizeDelegate authorizeDelegate) where TResource : notnull
    {
        var collectorResult = await authorizeDelegate(descriptor.Collector);
        
        if (!authorizeResources)
        {
            return new CollectionAuthorizationResult<TResource, TCollector>(
                collectorResult,
                descriptor.Resources.Select(x => (x, AuthorizationResult.Success())));
        }

        var results = new List<(TResource, AuthorizationResult)>();
        
        foreach (var resource in descriptor.Resources)
        {
            var result = await authorizeDelegate(resource);
            
            results.Add((resource, result));
        }

        return new CollectionAuthorizationResult<TResource, TCollector>(collectorResult, results);
    }
}