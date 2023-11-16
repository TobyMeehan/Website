using SqlKata;
using SqlKata.Execution;
using TobyMeehan.Com.Data.DataAccess;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Data.SqlKata;

public class UserRepository : Repository<UserDto>, IUserRepository
{
    public UserRepository(ISqlDataAccess db) : base(db, "users")
    {
    }

    protected override Query Query()
    {
        return base.Query()
            .OrderBy("Name")
            .Select();
    }

    public IAsyncEnumerable<UserDto> SelectByRoleAsync(string roleId, LimitStrategy? limit, CancellationToken ct)
    {
        var roles = new Query("userroles").Select("UserId").Where("Id", roleId);

        return Db.QueryAsync<UserDto>(Query(limit)
                .WhereIn(Column("Id"), roles), 
            cancellationToken: ct);
    }

    public async Task<UserDto?> SelectByUsernameAsync(string username, CancellationToken ct)
    {
        return await Db.SingleAsync<UserDto>(Query()
                .Where(Column("Handle"), username), 
            cancellationToken: ct);
    }
}