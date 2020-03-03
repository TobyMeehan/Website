using DataAccessLibrary.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLibrary.Storage
{
    public interface IRoleTable
    {
        Task Delete(string roleid);
        Task Insert(string name);
        Task<List<Role>> Select();
        Task<List<Role>> SelectById(string roleid);
        Task<List<Role>> SelectByName(string name);
    }
}