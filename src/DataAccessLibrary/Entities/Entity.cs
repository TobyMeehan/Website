namespace TobyMeehan.Com.Data.Entities;

public class Entity<T> : IEntity<T>
{
    public Entity(string id)
    {
        Id = new Id<T>(id);
    }

    public Id<T> Id { get; }
}