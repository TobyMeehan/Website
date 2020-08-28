using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Sql;

namespace TobyMeehan.Com.Data.Repositories
{
    public class SqlConversationRepository : SqlRepository<Conversation>
    {
        private readonly IUserRepository _users;

        public SqlConversationRepository(ISqlTable<Conversation> table, IUserRepository users) : base(table)
        {
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
    }
}
