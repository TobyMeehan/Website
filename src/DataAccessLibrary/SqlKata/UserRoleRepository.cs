using TobyMeehan.Com.Data.DataAccess;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories;

namespace TobyMeehan.Com.Data.SqlKata;

public class UserRoleRepository : Repository<RoleDto>, IUserRoleRepository
{
    public UserRoleRepository(ISqlDataAccess db) : base(db, "userroles")
    {
    }
}