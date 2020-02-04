using DataAccessLibrary.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLibrary.Storage
{
    public interface IApplicationTable
    {
        Task Delete(string appid);
        Task Insert(ApplicationModel app);
        Task<List<ApplicationModel>> SelectById(string appid);
        Task<List<ApplicationModel>> SelectByName(string name);
        Task<List<ApplicationModel>> SelectByUser(string userid);
        Task<List<ApplicationModel>> SelectByUserAndName(string userid, string name);
    }
}