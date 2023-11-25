using TobyMeehan.Com.Data.DataAccess;
using TobyMeehan.Com.Data.Domain.Authorizations.Models;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Data.Domain.Authorizations.Repositories;

public class AuthorizationRepository : Repository<AuthorizationDto>, IAuthorizationRepository
{
    public AuthorizationRepository(ISqlDataAccess db) : base(db, "authorizations")
    {
    }

    public IAsyncEnumerable<AuthorizationDto> SelectByApplicationAsync(string applicationId, LimitStrategy? limit,
        CancellationToken cancellationToken)
    {
        return Db.QueryAsync<AuthorizationDto>(Query(limit)
                .Where(Column("ApplicationId"), applicationId),
            cancellationToken);
    }

    public IAsyncEnumerable<AuthorizationDto> SelectByUserAsync(string userId, LimitStrategy? limit, CancellationToken cancellationToken)
    {
        return Db.QueryAsync<AuthorizationDto>(Query(limit)
                .Where(Column("UserId"), userId),
            cancellationToken);
    }

    public IAsyncEnumerable<AuthorizationDto> SelectByApplicationAndUserAsync(string applicationId, string userId, LimitStrategy? limit,
        CancellationToken cancellationToken)
    {
        return Db.QueryAsync<AuthorizationDto>(Query(limit)
                .Where(Column("ApplicationId"), applicationId)
                .Where(Column("UserId"), userId),
            cancellationToken);
    }

    public async Task DeleteByCreationAsync(DateTime threshold, CancellationToken cancellationToken)
    {
        await Db.ExecuteAsync(Query()
                .AsDelete()
                .Where(Column("CreatedAt"), ">", threshold),
            cancellationToken);
    }
}