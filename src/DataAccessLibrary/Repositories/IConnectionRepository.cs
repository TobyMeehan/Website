using TobyMeehan.Com.Data.Repositories.Models;

namespace TobyMeehan.Com.Data.Repositories;

public interface IConnectionRepository : IRepository<ConnectionData>
{
    Task<List<ConnectionData>> SelectByUserAsync(string userId, CancellationToken cancellationToken);

    Task<ConnectionData?> SelectByUserAndApplicationAsync(string userId, string applicationId, CancellationToken cancellationToken);
}