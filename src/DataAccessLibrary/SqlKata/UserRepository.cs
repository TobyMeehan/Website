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

    private const string Roles = "userroles";
    private readonly Query _roles = new Query(Roles)
        .OrderBy("Name");

    private const string UserRoles = "users_userroles";
    private readonly Query _userRoles = new Query(UserRoles);
    
    protected override Query Query()
    {
        return base.Query()
            .OrderBy("DisplayName")
            
            .LeftJoin(_userRoles.As(UserRoles), j => j.On($"{UserRoles}.UserId", $"{Table}.Id"))
            .LeftJoin(_roles.As(Roles), j => j.On($"{Roles}.Id", $"{UserRoles}.RoleId"))
            
            .Select($"{Table}.{{Id, Username, DisplayName, HashedPassword, Balance, Description}}",
                $"{Roles}.Id AS Roles_Id", $"{Roles}.Name AS Roles_Name");
    }

    public IAsyncEnumerable<UserDto> SelectByRoleAsync(string roleId, LimitStrategy? limit, CancellationToken ct)
    {
        var roles = _userRoles.Select("UserId").Where("Id", roleId);

        return Db.QueryAsync<UserDto>(Query(limit)
                .WhereIn(Column("Id"), roles), 
            cancellationToken: ct);
    }

    public async Task<UserDto?> SelectByUsernameAsync(string username, CancellationToken ct)
    {
        return await Db.SingleAsync<UserDto>(Query()
                .Where(Column("Username"), username), 
            cancellationToken: ct);
    }

    public async Task<int> AddRoleAsync(string id, string roleId, CancellationToken ct)
    {
        return await Db.ExecuteAsync(_userRoles.AsInsert(new UserRoleDto
        {
            UserId = id,
            RoleId = roleId
        }), ct);
    }

    public async Task<int> RemoveRoleAsync(string id, string roleId, CancellationToken ct)
    {
        return await Db.ExecuteAsync(_userRoles
                .AsDelete()
                .Where("UserId", id)
                .Where("RoleId", roleId),
            ct);
    }
}