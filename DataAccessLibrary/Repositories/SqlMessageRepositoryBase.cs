using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Sql;

namespace TobyMeehan.Com.Data.Repositories
{
    public abstract class SqlMessageRepositoryBase<TMessage> : SqlRepository<TMessage> where TMessage : MessageBase
    {
        private readonly ISqlTable<TMessage> _table;
        private readonly IUserRepository _users;

        public SqlMessageRepositoryBase(ISqlTable<TMessage> table, IUserRepository users) : base (table)
        {
            _table = table;
            _users = users;
        }

        protected override async Task<IEnumerable<TMessage>> FormatAsync(IEnumerable<TMessage> values)
        {
            foreach (var message in values)
            {
                message.User = await _users.GetByIdAsync(message.UserId);
            }

            return await base.FormatAsync(values);
        }

        protected async Task<TMessage> AddAsync(string userId, string content, object additionalData = null)
        {
            string id = Guid.NewGuid().ToString();

            await _table.InsertAsync(new
            {
                Id = id,
                UserId = userId,
                Content = content,
                Sent = DateTime.Now
            });

            if (additionalData != null)
            {
                await _table.UpdateAsync(x => x.Id == id, additionalData);
            }

            return await GetByIdAsync(id);
        }

        protected async Task UpdateAsync(string id, string content, object additionalData = null)
        {
            await _table.UpdateAsync(x => x.Id == id, new
            {
                Content = content,
                Edited = DateTime.Now
            });

            await _table.UpdateAsync(x => x.Id == id, additionalData);
        }
    }
}
