using SqlKata;
using SqlKata.Execution;
using TobyMeehan.Com.Data.DataAccess;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Data.SqlKata;

public class TokenRepository : Repository<TokenDto>, ITokenRepository
{
    public TokenRepository(ISqlDataAccess db) : base(db, "tokens")
    {
    }

    public IAsyncEnumerable<TokenDto> SelectByApplicationAsync(string applicationId, LimitStrategy? limit, CancellationToken ct)
    {
        return Db.QueryAsync<TokenDto>(Query(limit)
                .Where(Column("ApplicationId"), applicationId), 
            cancellationToken: ct);
    }

    public IAsyncEnumerable<TokenDto> SelectByUserAsync(string userId, LimitStrategy? limit, CancellationToken ct)
    {
        return Db.QueryAsync<TokenDto>(Query(limit)
                .Where(Column("UserId"), userId), 
            cancellationToken: ct);
    }

    public IAsyncEnumerable<TokenDto> SelectByApplicationAndUserAsync(string applicationId, string userId, 
        LimitStrategy? limit, CancellationToken ct)
    {
        return Db.QueryAsync<TokenDto>(Query(limit)
                .Where(Column("ApplicationId"), applicationId)
                .Where(Column("UserId"), userId),
            cancellationToken: ct);
    }

    public IAsyncEnumerable<TokenDto> SelectByAuthorizationAsync(string authorizationId, LimitStrategy? limit, CancellationToken ct)
    {
        return Db.QueryAsync<TokenDto>(Query(limit)
                .Where(Column("AuthorizationId"), authorizationId),
            cancellationToken: ct);
    }

    public async Task<TokenDto?> SelectByReferenceIdAsync(string referenceId, CancellationToken ct)
    {
        return await Db.SingleAsync<TokenDto>(Query()
                .Where(Column("ReferenceId"), referenceId),
            cancellationToken: ct);
    }

    public async Task DeleteByExpirationAsync(DateTime threshold, CancellationToken ct)
    {
        await Db.ExecuteAsync(Query()
                .AsDelete()
                .Where(Column("ExpiresAt"), "<", threshold),
            cancellationToken: ct);
    }
}