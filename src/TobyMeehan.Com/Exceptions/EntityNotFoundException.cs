namespace TobyMeehan.Com.Exceptions;

/// <summary>
/// Exception thrown when an attempt to access an entity using an <see cref="Id"/> failed.
/// </summary>
/// <typeparam name="T"></typeparam>
public class EntityNotFoundException<T> : Exception where T : IEntity<T>
{
    /// <summary>
    /// The <see cref="Id"/> used in the failed operation.
    /// </summary>
    public Id<T> Id { get; }

    /// <summary>
    /// Creates a new instance of <see cref="EntityNotFoundException{T}"/> with the specified ID and a default message.
    /// </summary>
    /// <param name="id"></param>
    public EntityNotFoundException(Id<T> id) : base($"Entity with ID {id.Value} does not exist.")
    {
        Id = id;
    }
}