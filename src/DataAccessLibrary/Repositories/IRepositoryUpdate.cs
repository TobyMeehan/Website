namespace TobyMeehan.Com.Data.Repositories;

public interface IRepositoryUpdate<in T>
{
    Task UpdateAsync(string id, T data, CancellationToken cancellationToken);
}