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

    public SessionService(ISessionRepository db, IIdService id, ISecretService secretService, IConnectionService connections) : base(db)
    {
        _db = db;
        _id = id;
        _secretService = secretService;
        _connections = connections;
    }

    protected override async Task<ISession> MapperAsync(SessionData data)
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
        
        return new Session(data.Id, connection, redirect, data.AuthorizationCode, data.Scope?.Split() ?? Enumerable.Empty<string>(),
            data.CodeChallenge, data.RefreshToken, data.Expiry);
    }

    protected override async Task<(Id<ISession>, SessionData)> CreateAsync(CreateSessionBuilder create)
    {
        var id = await _id.GenerateAsync<ISession>();
        string code = await _secretService.GenerateSecretAsync(32);

        return (id, new SessionData
        {
            Id = id.Value,
            ConnectionId = create.Connection.Value,
            RedirectId = create.Redirect.Value,
            AuthorizationCode = code,
            Scope = create.Scope,
            CodeChallenge = create.CodeChallenge,
            RefreshToken = null,
            Expiry = DateTime.UtcNow.AddMinutes(10)
        });
    }

    public async Task<ISession?> GetByCodeAsync(string code, CancellationToken ct)
    {
        var data = await _db.SelectByCodeAsync(code, ct);

        return await MapAsync(data);
    }
    
    public async Task<ISession?> RefreshAsync(string refreshToken, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public async Task<ISession> StartAsync(Id<ISession> id, StartSessionBuilder session, CancellationToken cancellationToken = default)
    {
        string refreshToken = await _secretService.GenerateSecretAsync(32);

        return await UpdateAsync(id, data =>
        {
            data.Expiry = DateTime.UtcNow.AddMonths(6);
            data.RefreshToken = session.CanRefresh ? refreshToken : null;
        }, cancellationToken);
    }

    public async Task DeleteByConnectionAsync(Id<IConnection> connection, CancellationToken ct)
    {
        await _db.DeleteByConnectionAsync(connection.Value, ct);
    }
}