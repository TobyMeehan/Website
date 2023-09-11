using TobyMeehan.Com.Builders;

namespace TobyMeehan.Com.Services;

/// <summary>
/// Service for OAuth session data.
/// </summary>
public interface ISessionService
{
    Task<ISession> CreateAsync(CreateSessionBuilder session, CancellationToken cancellationToken = default);

    Task<IEntityCollection<ISession>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<ISession> GetByIdAsync(Id<ISession> id, CancellationToken cancellationToken = default);

    Task<ISession?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    Task<ISession?> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default);

    Task DeleteAsync(Id<ISession> id, CancellationToken cancellationToken = default);

    Task DeleteByConnectionAsync(Id<IConnection> connection, CancellationToken cancellationToken = default);
}