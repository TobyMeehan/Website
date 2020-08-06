using System.Collections.Generic;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Models;

namespace TobyMeehan.Com.Data.Repositories
{
    public interface ITransactionRepository
    {
        Task<Transaction> AddAsync(string userId, string appId, string description, int amount);
        Task<IList<Transaction>> GetAsync();
        Task<Transaction> GetByIdAsync(string id);
        Task<IList<Transaction>> GetByUserAsync(string userId);
    }
}