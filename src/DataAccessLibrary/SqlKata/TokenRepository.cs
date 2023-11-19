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

    private const string Authorizations = "authorizations";

    private readonly Query _authorizations = new Query(Authorizations);
    
    protected override Query Query()
    {
        return base.Query()
            .LeftJoin(_authorizations.As(Authorizations), j => j.On($"{Table}.AuthorizationId", $"{Authorizations}.Id"))

            .Select(
                $"{Table}.{{Id, AuthorizationId, ReferenceId, Payload, Type, Status, RedeemedAt, ExpiresAt, CreatedAt}}",
                $"{Authorizations}.ApplicationId AS ApplicationId");
    }

    public IAsyncEnumerable<TokenDto> SelectByApplicationAsync(string applicationId, LimitStrategy? limit, CancellationToken ct)
    {
        return Db.QueryAsync<TokenDto>(Query(limit)
                .Where("ApplicationId", applicationId), 
            cancellationToken: ct);
    }

    public IAsyncEnumerable<TokenDto> SelectBySubjectAsync(string subject, LimitStrategy? limit, CancellationToken ct)
    {
        return Db.QueryAsync<TokenDto>(Query(limit)
                .Where(Column("Subject"), subject), 
            cancellationToken: ct);
    }

    public IAsyncEnumerable<TokenDto> SelectByApplicationAndSubjectAsync(string applicationId, string? subject, 
        LimitStrategy? limit, CancellationToken ct)
    {
        return Db.QueryAsync<TokenDto>(Query(limit)
                .Where("ApplicationId", applicationId)
                .Where(Column("Subject"), subject),
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