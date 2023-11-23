namespace TobyMeehan.Com.Api.CollectionAuthorization;

public struct ResourceCollectionDescriptor<TResource, TCollector>
{
    public ResourceCollectionDescriptor(IEnumerable<TResource> resources, TCollector collector)
    {
        Resources = resources;
        Collector = collector;
    }

    public IEnumerable<TResource> Resources { get; }
    public TCollector Collector { get; }
}