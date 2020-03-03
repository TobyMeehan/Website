using DataAccessLibrary.Models;
using System.Threading.Tasks;

namespace DataAccessLibrary.Data
{
    public interface IRoleProcessor
    {
        Task CreateRole(Role role);
        Task DeleteRole(Role role);
        Task<Role> GetRoleById(string roleid);
        Task<Role> GetRoleByName(string name);
    }
}