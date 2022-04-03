namespace TobyMeehan.Com.Data;

public interface IIdGenerator
{
    Id<T> GenerateId<T>();
}