using TobyMeehan.Com.Data.Repositories.Models;

namespace TobyMeehan.Com.Data.Repositories;

public interface ISessionRepository : IRepository<SessionData>
{
    Task<SessionData?> SelectByCodeAsync(string code, CancellationToken cancellationToken);

    Task<SessionData?> SelectByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);

    Task DeleteByConnectionAsync(string connectionId, CancellationToken cancellationToken);
}