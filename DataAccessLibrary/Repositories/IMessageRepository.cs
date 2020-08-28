using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Models;

namespace TobyMeehan.Com.Data.Repositories
{
    public interface IMessageRepository
    {
        Task<Message> GetByIdAsync(string id);

        Task<IList<Message>> GetByConversationAsync(string conversationId);

        Task<Message> AddAsync(string conversationId, string userId, string content);

        Task UpdateAsync(string id, string content);

        Task DeleteAsync(string id);
    }
}
