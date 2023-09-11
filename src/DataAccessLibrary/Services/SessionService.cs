using TobyMeehan.Com.Builders;
using TobyMeehan.Com.Data.Entities;
using TobyMeehan.Com.Data.Repositories;
using TobyMeehan.Com.Data.Repositories.Models;
using TobyMeehan.Com.Data.Security;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Data.Services;

public class SessionService : BaseService<ISession, SessionData, CreateSessionBuilder>, ISessionService
{
    private readonly ISessionRepository _db;
    private readonly IIdService _id;
    private readonly ISecretService _secretService;

    public SessionService(ISessionRepository db, IIdService id, ISecretService secretService) : base(db)
    {
        _db = db;
        _id = id;
        _secretService = secretService;
    }

    protected override async Task<ISession> MapperAsync(SessionData data)
    {
        return new Session(data.Id, data.ConnectionId, data.RedirectId, data.AuthorizationCode, data.Scope?.Split() ?? Enumerable.Empty<string>(),
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

    public async Task DeleteByConnectionAsync(Id<IConnection> connection, CancellationToken ct)
    {
        await _db.DeleteByConnectionAsync(connection.Value, ct);
    }
}