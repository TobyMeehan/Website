using DataAccessLibrary.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLibrary.Data
{
    public interface IApplicationProcessor
    {
        Task<ApplicationModel> CreateApplication(ApplicationModel app, bool secret);
        Task DeleteApplication(string appid);
        Task<ApplicationModel> GetApplicationById(string appid);
        Task<ApplicationModel> GetApplicationByUserAndName(string userid, string name);
        Task<List<ApplicationModel>> GetApplicationsByName(string name);
        Task<List<ApplicationModel>> GetApplicationsByUser(string userid);
        Task UpdateApplication(ApplicationModel app);
    }
}