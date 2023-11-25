using TobyMeehan.Com.Data.DataAccess;
using TobyMeehan.Com.Data.Domain.UserRoles.Models;

namespace TobyMeehan.Com.Data.Domain.UserRoles.Repositories;

public class UserRoleRepository : Repository<RoleDto>, IUserRoleRepository
{
    public UserRoleRepository(ISqlDataAccess db) : base(db, "userroles")
    {
    }
}