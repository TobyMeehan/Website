using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Models;

namespace TobyMeehan.Com.Data.Repositories
{
    public interface IApplicationRepository
    {
        Task<IList<Application>> GetAsync();

        Task<Application> GetByIdAsync(string id);

        Task<IList<Application>> GetByUserAsync(string userId);

        Task<IList<Application>> GetByNameAsync(string name);

        Task<Application> GetByUserAndNameAsync(string userId, string name);

        Task<Application> AddAsync(string userId, string name, string redirectUri, string secret = null);

        Task UpdateAsync(Application application);

        Task DeleteAsync(string id);

        Task<bool> ValidateAsync(string id, string secret, string redirectUri, bool ignoreSecret);
    }
}
