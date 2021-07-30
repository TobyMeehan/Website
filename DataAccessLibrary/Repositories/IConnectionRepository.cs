using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Collections;
using TobyMeehan.Com.Data.Models;

namespace TobyMeehan.Com.Data.Repositories
{
    public interface IConnectionRepository
    {
        Task<IEntityCollection<Connection>> GetAsync();

        Task<IEntityCollection<Connection>> GetByUserAsync(string userId);

        Task<IEntityCollection<Connection>> GetByApplicationAsync(string appId);

        Task<Connection> GetByIdAsync(string id);

        Task<Connection> GetOrCreateAsync(string userId, string appId);

        Task DeleteAsync(string id);
    }
}
