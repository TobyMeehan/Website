using TobyMeehan.Com.Data.DataAccess;
using TobyMeehan.Com.Data.Domain.Avatars.Models;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Data.Domain.Avatars.Repositories;

public class AvatarRepository : Repository<AvatarDto>, IAvatarRepository
{
    public AvatarRepository(ISqlDataAccess db) : base(db, "avatars")
    {
    }

    public IAsyncEnumerable<AvatarDto> SelectByUserAsync(string userId, LimitStrategy? limit, CancellationToken cancellationToken)
    {
        return Db.QueryAsync<AvatarDto>(Query(limit)
                .Where(Column("UserId"), userId),
            cancellationToken: cancellationToken);
    }

    public async Task<int> DeleteByUserAsync(string userId, CancellationToken cancellationToken)
    {
        return await Db.ExecuteAsync(Query()
                .AsDelete()
                .Where(Column("UserId"), userId), 
            cancellationToken);
    }
}