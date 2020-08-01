using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Models;

namespace TobyMeehan.Com.Data.Repositories
{
    public interface IConnectionRepository
    {
        Task<IList<Connection>> GetAsync();

        Task<IList<Connection>> GetByUserAsync(string userId);

        Task<IList<Connection>> GetByApplicationAsync(string appId);

        Task<Connection> GetByIdAsync(string id);

        Task<Connection> GetOrCreateAsync(string userId, string appId);

        Task DeleteAsync(string id);
    }
}
