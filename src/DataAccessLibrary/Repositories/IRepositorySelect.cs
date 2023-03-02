namespace TobyMeehan.Com.Data.Repositories;

public interface IRepositorySelect<T>
{
    Task<List<T>> SelectAllAsync(CancellationToken cancellationToken);

    Task<T?> SelectByIdAsync(string id, CancellationToken cancellationToken);
}