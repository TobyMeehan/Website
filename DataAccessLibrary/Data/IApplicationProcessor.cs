using DataAccessLibrary.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLibrary.Data
{
    public interface IApplicationProcessor
    {
        Task<Application> CreateApplication(Application app, bool secret);
        Task DeleteApplication(string appid);
        Task<Application> GetApplicationById(string appid);
        Task<Application> GetApplicationByUserAndName(string userid, string name);
        Task<List<Application>> GetApplicationsByName(string name);
        Task<List<Application>> GetApplicationsByUser(string userid);
        Task UpdateApplication(Application app);
        Task<bool> ValidateApplication(string clientId, string clientSecret, string redirectUri);
    }
}