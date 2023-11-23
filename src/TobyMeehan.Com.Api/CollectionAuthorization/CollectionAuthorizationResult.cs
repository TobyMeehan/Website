using System.Collections;
using Microsoft.AspNetCore.Authorization;

namespace TobyMeehan.Com.Api.CollectionAuthorization;

public class CollectionAuthorizationResult<TResource> : IEnumerable<(TResource Resource, AuthorizationResult Result)> where TResource : notnull
{
    public CollectionAuthorizationResult(
        AuthorizationResult collectorResult, 
        IEnumerable<(TResource, AuthorizationResult)> resourceResults)
    {
        CollectorResult = collectorResult;
        ResourceResults = resourceResults
            .ToDictionary(key => key.Item1, value => value.Item2)
            .AsReadOnly();
    }
    
    public AuthorizationResult CollectorResult { get; }

    public bool Succeeded => CollectorResult.Succeeded;

    public AuthorizationFailure? Failure => CollectorResult.Failure;

    public IReadOnlyDictionary<TResource, AuthorizationResult> ResourceResults { get; }

    public IEnumerable<TResource> AuthorizedResources => ResourceResults
        .Where(x => x.Value.Succeeded)
        .Select(x => x.Key);

    public IEnumerator<(TResource Resource, AuthorizationResult Result)> GetEnumerator()
    {
        return ResourceResults
            .Select(x => (x.Key, x.Value))
            .GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}