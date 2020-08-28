using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Sql;

namespace TobyMeehan.Com.Data.Repositories
{
    public class SqlMessageRepository : SqlMessageRepositoryBase<Message>, IMessageRepository
    {
        public SqlMessageRepository(ISqlTable<Message> sqlTable, IUserRepository users) : base(sqlTable, users)
        {

        }

        public Task<Message> AddAsync(string conversationId, string userId, string content)
        {
            return AddAsync(userId, content, new
            {
                ConversationId = conversationId
            });
        }

        public async Task<IList<Message>> GetByConversationAsync(string conversationId)
        {
            return (await SelectAsync(m => m.ConversationId == conversationId)).ToList();
        }

        public Task UpdateAsync(string id, string content)
        {
            return UpdateAsync(id, content);
        }
    }
}
