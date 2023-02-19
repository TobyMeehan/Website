namespace TobyMeehan.Com.Data.Repositories;

public interface IRepositoryInsert<in T>
{
    Task InsertAsync(T data, CancellationToken cancellationToken);
}