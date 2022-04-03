using SqlKata;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Extensions;

namespace TobyMeehan.Com.Data.SqlKata
{
    public class CommentRepository : RepositoryBase<IComment, Comment, NewComment, EditComment>, ICommentRepository
    {
        private readonly IUserRepository _users;

        public CommentRepository(QueryFactory queryFactory, IIdGenerator idGenerator, IUserRepository users) 
            : base(queryFactory, idGenerator, "comments")
        {
            _users = users;
        }

        protected override Query Query()
        {
            return base.Query()
                .From(Table)
                .OrderBy("Sent");

        }

        protected override async Task<IReadOnlyList<IComment>> MapAsync(IEnumerable<Comment> items)
        {
            var list = items.ToList();
            
            foreach (var item in list)
            {
                item.User = await _users.GetByIdAsync(item.UserId);
            }

            return await base.MapAsync(list);
        }

        public Task<IReadOnlyList<IComment>> GetByEntityAsync(string entityId)
        {
            return SelectAsync(query => query.Where($"{Table}.EntityId", entityId));
        }
    }
}
