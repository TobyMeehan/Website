using Microsoft.Extensions.Caching.Memory;
using TobyMeehan.Com.Builders;
using TobyMeehan.Com.Data.Entities;
using TobyMeehan.Com.Data.Repositories;
using TobyMeehan.Com.Data.Repositories.Models;
using TobyMeehan.Com.Data.Security;
using TobyMeehan.Com.Exceptions;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Data.Services;

public class SessionService : BaseService<ISession, SessionData, CreateSessionBuilder>, ISessionService
{
    private readonly ISessionRepository _db;
    private readonly IIdService _id;
    private readonly ISecretService _secretService;
    private readonly IConnectionService _connections;

    public SessionService(ISessionRepository db, IIdService id, ISecretService secretService, IConnectionService connections, IMemoryCache cache) : base(db, cache)
    {
        _db = db;
        _id = id;
        _secretService = secretService;
        _connections = connections;
    }

    protected override async Task<ISession> MapAsync(SessionData data)
    {
        var connectionId = new Id<IConnection>(data.ConnectionId);
        
        var connection = await _connections.GetByIdAsync(connectionId);

        if (connection is null)
        {
            throw new EntityNotFoundException<IConnection>(connectionId);
        }

        IRedirect? redirect = null;
        
        if (data.RedirectId is not null)
        {
            var redirectId = new Id<IRedirect>(data.RedirectId);

            redirect = connection.Application.Redirects[redirectId];
        }
        
        return new Session(data.Id, connection, redirect, data.Scope?.Split() ?? Enumerable.Empty<string>(), data.RefreshToken, data.Expiry);
    }

    protected override async Task<SessionData> CreateAsync(CreateSessionBuilder create)
    {
        var id = await _id.GenerateAsync<ISession>();
        
        return new SessionData
        {
            Id = id.Value,
            ConnectionId = create.Connection.Value,
            RedirectId = create.Redirect.Value,
            Scope = create.Scope,
            RefreshToken = create.CanRefresh ? await _secretService.GenerateSecretAsync(32) : null,
            Expiry = DateTime.UtcNow.AddMonths(6)
        };
    }

    public async Task<ISession?> FindByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var data = await _db.SelectByRefreshTokenAsync(refreshToken, cancellationToken);

        if (data is null)
        {
            return null;
        }
        
        return await MapAsync(data);
    }

    public async Task<ISession> RefreshAsync(Id<ISession> id, string? scope, CancellationToken cancellationToken)
    {
        var session = await GetByIdAsync(id, cancellationToken);

        if (session is null)
        {
            throw new EntityNotFoundException<ISession>(id);
        }

        string refreshToken = await _secretService.GenerateSecretAsync(32);
        
        return await UpdateAsync(id, data =>
        {
            data.Scope = scope;
            data.RefreshToken = refreshToken;
            data.Expiry = DateTime.UtcNow.AddMonths(6);
        }, cancellationToken);
    }

    public async Task DeleteByConnectionAsync(Id<IConnection> connection, CancellationToken ct)
    {
        await _db.DeleteByConnectionAsync(connection.Value, ct);
    }
}