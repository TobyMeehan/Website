using System.Collections.Generic;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Collections;
using TobyMeehan.Com.Data.Models;

namespace TobyMeehan.Com.Data.Repositories
{
    public interface ITransactionRepository
    {
        Task<Transaction> AddAsync(string userId, string appId, string description, int amount);
        Task<IEntityCollection<Transaction>> GetAsync();
        Task<Transaction> GetByIdAsync(string id);
        Task<int> TotalPagesForUserAsync(string userId, int perPage = 15);
        Task<IEntityCollection<Transaction>> GetByUserAsync(string userId, int page = 1, int perPage = 15);
    }
}