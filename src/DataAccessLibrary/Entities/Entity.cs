namespace TobyMeehan.Com.Data.Entities;

public class Entity<T> : IEntity<T>
{
    public Id<T> Id { get; }
}