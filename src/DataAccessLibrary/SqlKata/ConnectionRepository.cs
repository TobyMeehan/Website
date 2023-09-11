using SqlKata;
using SqlKata.Execution;
using TobyMeehan.Com.Data.Repositories;
using TobyMeehan.Com.Data.Repositories.Models;

namespace TobyMeehan.Com.Data.SqlKata;

public class ConnectionRepository : Repository<ConnectionData>, IConnectionRepository
{
    public ConnectionRepository(QueryFactory db) : base(db, "connections")
    {
    }

    public async Task<List<ConnectionData>> SelectByUserAsync(string userId, CancellationToken ct)
    {
        return await QueryAsync(query => query.Where($"{Table}.UserId", userId), cancellationToken: ct);
    }

    public async Task<ConnectionData?> SelectByUserAndApplicationAsync(string userId, string applicationId, CancellationToken ct)
    {
        return await QuerySingleAsync(
            query => query.Where($"{Table}.UserId", userId).Where("ApplicationId", applicationId),
            cancellationToken: ct);
    }
}