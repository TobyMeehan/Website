using SqlKata.Execution;
using TobyMeehan.Com.Data.Repositories;
using TobyMeehan.Com.Data.Repositories.Models;

namespace TobyMeehan.Com.Data.SqlKata;

public class SessionRepository : Repository<SessionData>, ISessionRepository
{
    public SessionRepository(QueryFactory db) : base(db, "sessions")
    {
    }
    
    public async Task<SessionData?> SelectByCodeAsync(string code, CancellationToken ct)
    {
        return await QuerySingleAsync(query => query.Where($"{Table}.AuthorizationCode", code), ct);
    }

    public async Task<SessionData?> SelectByRefreshTokenAsync(string refreshToken, CancellationToken ct)
    {
        return await QuerySingleAsync(query => query.Where($"{Table}.RefreshToken", refreshToken), ct);
    }

    public async Task DeleteByConnectionAsync(string connectionId, CancellationToken ct)
    {
        await Db.Query(Table).Where("ConnectionId", connectionId).DeleteAsync(cancellationToken: ct);
    }
}