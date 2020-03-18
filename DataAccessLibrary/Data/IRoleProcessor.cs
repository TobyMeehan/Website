using DataAccessLibrary.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLibrary.Data
{
    public interface IRoleProcessor
    {
        Task CreateRole(string name);
        Task DeleteRole(Role role);
        Task<Role> GetRoleById(string roleid);
        Task<Role> GetRoleByName(string name);
        Task<List<Role>> GetRoles();
    }
}