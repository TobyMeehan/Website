namespace TobyMeehan.Com.Data.Repositories;

public interface IRepository<T> : IRepositorySelect<T>, IRepositoryInsert<T>, IRepositoryUpdate<T>, IRepositoryDelete
{
    
}