using DataAccessLibrary.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLibrary.Storage
{
    public interface IApplicationTable
    {
        Task Delete(string appid);
        Task Insert(Application app);
        Task<List<Application>> SelectById(string appid);
        Task<List<Application>> SelectByName(string name);
        Task<List<Application>> SelectByUser(string userid);
        Task<List<Application>> SelectByUserAndName(string userid, string name);
        Task Update(Application app);
    }
}