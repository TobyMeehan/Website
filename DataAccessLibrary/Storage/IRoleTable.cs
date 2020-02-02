using DataAccessLibrary.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLibrary.Storage
{
    public interface IRoleTable
    {
        Task Delete(int roleid);
        Task Insert(string name);
        Task<List<RoleModel>> Select();
        Task<List<RoleModel>> SelectById(int roleid);
        Task<List<RoleModel>> SelectByName(string name);
    }
}