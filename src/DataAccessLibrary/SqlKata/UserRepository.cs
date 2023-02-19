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

    protected override Query Query()
    {
        return base.Query()
            .OrderBy("Name")
            .Select();
    }

    public async Task<List<UserData>> SelectByRoleAsync(string roleId, CancellationToken ct)
    {
        var roles = new Query("userroles").Select("UserId").Where("Id", roleId);

        return await QueryAsync(query => query.WhereIn("users.Id", roles), cancellationToken: ct);
    }

    public async Task<UserData> SelectByHandleAsync(string handle, CancellationToken ct)
    {
        return await QuerySingleAsync(query => query.Where("Handle", handle), cancellationToken: ct);
    }
}