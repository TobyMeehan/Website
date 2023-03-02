namespace TobyMeehan.Com.Exceptions;

public class EntityNotFoundException<T> : Exception where T : IEntity<T>
{
    public Id<T> Id { get; }

    public EntityNotFoundException(Id<T> id) : base($"Entity with ID {id.Value} does not exist.")
    {
        Id = id;
    }
}