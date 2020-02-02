using DataAccessLibrary.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLibrary.Storage
{
    public interface IUserRoleTable
    {
        Task DeleteByRole(string roleid);
        Task DeleteByUser(string userid);
        Task Insert(UserRoleModel value);
        Task<List<UserRoleModel>> SelectByRole(string roleid);
        Task<List<UserRoleModel>> SelectByUser(string userid);
    }
}