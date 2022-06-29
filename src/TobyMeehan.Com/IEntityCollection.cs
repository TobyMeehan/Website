namespace TobyMeehan.Com;

/// <summary>
/// Collection of entities, indexed by ID.
/// </summary>
/// <typeparam name="T">Type of entity.</typeparam>
public interface IEntityCollection<T> : IReadOnlyCollection<T> where T : IEntity<T>
{
    /// <summary>
    /// Gets the entity in the collection with the specified ID, null otherwise.
    /// </summary>
    /// <param name="id"></param>
    T? this[Id<T> id] { get; }
}