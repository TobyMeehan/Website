using DataAccessLibrary.Models;
using System.Threading.Tasks;

namespace DataAccessLibrary.Data
{
    public interface IRoleProcessor
    {
        Task CreateRole(RoleModel role);
        Task DeleteRole(RoleModel role);
        Task<RoleModel> GetRoleById(int roleid);
        Task<RoleModel> GetRoleByName(string name);
    }
}