using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Sql;

namespace TobyMeehan.Com.Data.Repositories
{
    public class SqlCommentRepository : SqlMessageRepositoryBase<Comment>, ICommentRepository
    {
        private readonly ISqlTable<Comment> _sqlTable;

        public SqlCommentRepository(ISqlTable<Comment> sqlTable, IUserRepository users) : base (sqlTable, users)
        {
            _sqlTable = sqlTable;
        }

        public Task<Comment> AddAsync(string entityId, string userId, string content)
        {
            return AddAsync(userId, content, new
            {
                EntityId = entityId
            });
        }

        public async Task<IList<Comment>> GetByEntityAsync(string entityId)
        {
            return (await SelectAsync(c => c.EntityId == entityId)).ToList();
        }

        public Task UpdateAsync(string id, string content)
        {
            return UpdateAsync(id, content);
        }
    }
}
