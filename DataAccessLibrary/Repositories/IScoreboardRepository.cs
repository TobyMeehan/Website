using System.Collections.Generic;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Collections;
using TobyMeehan.Com.Data.Models;

namespace TobyMeehan.Com.Data.Repositories
{
    public interface IScoreboardRepository
    {
        Task<Objective> AddAsync(string appId, string objectiveName);
        Task DeleteAsync(string id);
        Task<Objective> GetByIdAsync(string id);
        Task<IEntityCollection<Objective>> GetByApplicationAsync(string appId);
        Task SetScoreAsync(string id, string userId, int value);
    }
}