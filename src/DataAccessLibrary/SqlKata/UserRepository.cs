using SqlKata;
using SqlKata.Execution;
using TobyMeehan.Com.Data.Repositories;
using TobyMeehan.Com.Data.Repositories.Models;

namespace TobyMeehan.Com.Data.SqlKata;

public class UserRepository : Repository<UserData>, IUserRepository
{
    public UserRepository(QueryFactory db) : base(db, "users")
    {
    }

    public async Task<List<UserData>> SelectByRoleAsync(string roleId, CancellationToken cancellationToken = default)
    {
        var roles = new Query("userroles").Select("UserId").Where("Id", roleId);

        return await QueryAsync(query => query.WhereIn("users.Id", roles), cancellationToken: cancellationToken);
    }

    public async Task<UserData> SelectByHandleAsync(string handle, CancellationToken cancellationToken = default)
    {
        return await QuerySingleAsync(query => query.Where("Handle", handle), cancellationToken: cancellationToken);
    }
}