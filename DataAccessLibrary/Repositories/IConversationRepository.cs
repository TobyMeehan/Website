using System.Collections.Generic;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Models;

namespace TobyMeehan.Com.Data.Repositories
{
    public interface IConversationRepository
    {
        Task<Conversation> GetByIdAsync(string id);

        Task<List<Conversation>> GetByUserAsync(string userId);

        Task<Conversation> AddAsync(string name, string userId);
    }
}