namespace TobyMeehan.Com;

/// <summary>
/// Base interface for an entity.
/// </summary>
/// <typeparam name="T">Derived type of entity.</typeparam>
public interface IEntity<T>
{
    /// <summary>
    /// ID of the entity.
    /// </summary>
    Id<T> Id { get; }
}