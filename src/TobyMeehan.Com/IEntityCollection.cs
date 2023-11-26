using OneOf;
using TobyMeehan.Com.Results;

namespace TobyMeehan.Com;

/// <summary>
/// Collection of entities, indexed by ID.
/// </summary>
/// <typeparam name="T">Type of entity.</typeparam>
/// <typeparam name="TId">Type of ID of the entity.</typeparam>
public interface IEntityCollection<T, TId> : IReadOnlyCollection<T> where T : IEntity<TId>
{
    /// <summary>
    /// Gets the entity in the collection with the specified ID.
    /// </summary>
    /// <param name="id"></param>
    OneOf<T, NotFound> this[Id<TId> id] { get; }

    /// <summary>
    /// Finds an entity in the collection with the specified ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    T? Find(string id);
}

/// <summary>
/// Collection of entities, indexed by ID.
/// </summary>
/// <typeparam name="T">Type of entity.</typeparam>
public interface IEntityCollection<T> : IEntityCollection<T, T> where T : IEntity<T>
{
}
