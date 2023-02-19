namespace TobyMeehan.Com.Data;

public interface IIdService
{
    Task<Id<T>> GenerateAsync<T>();
}