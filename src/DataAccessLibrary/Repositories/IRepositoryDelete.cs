namespace TobyMeehan.Com.Data.Repositories;

public interface IRepositoryDelete
{
    Task DeleteAsync(string id, CancellationToken cancellationToken);
}