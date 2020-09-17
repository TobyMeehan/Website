using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Sql;

namespace TobyMeehan.Com.Data.Repositories
{
    public class SqlConversationRepository : SqlRepository<Conversation>, IConversationRepository
    {
        private readonly ISqlTable<Conversation> _table;
        private readonly ISqlTable<ConversationUser> _userTable;
        private readonly IUserRepository _users;

        public SqlConversationRepository(ISqlTable<Conversation> table, ISqlTable<ConversationUser> userTable, IUserRepository users) : base(table)
        {
            _table = table;
            _userTable = userTable;
            _users = users;
        }

        protected override async Task<IEnumerable<Conversation>> FormatAsync(IEnumerable<Conversation> values)
        {
            foreach (var conversation in values)
            {
                foreach (string user in conversation.Users.Select(x => x.Id))
                {
                    conversation.Users[user] = await _users.GetByIdAsync(user);
                }
            }

            return await base.FormatAsync(values);
        }

        public async Task<Conversation> AddAsync(string name, string userId)
        {
            string id = Guid.NewGuid().ToString();

            await _table.InsertAsync(new
            {
                Id = id,
                Name = name
            });

            await _userTable.InsertAsync(new
            {
                ConversationId = id,
                UserId = userId
            });

            return await GetByIdAsync(id);
        }

        public async Task<List<Conversation>> GetByUserAsync(string userId)
        {
            return (await SelectAsync<ConversationUser>((c, cu) => cu.UserId == userId)).ToList();
        }
    }
}
