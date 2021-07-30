using SqlKata;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Collections;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories;

namespace TobyMeehan.Com.Data.SqlKata
{
    public class CommentRepository : RepositoryBase<Comment>, ICommentRepository
    {
        private readonly Func<QueryFactory> _queryFactory;
        private readonly IUserRepository _users;

        public CommentRepository(Func<QueryFactory> queryFactory, IUserRepository users) : base(queryFactory)
        {
            _queryFactory = queryFactory;
            _users = users;
        }

        protected override Query Query()
        {
            return base.Query()
                .From("comments")
                .OrderBy("Sent");

        }

        protected override async Task<IEntityCollection<Comment>> MapAsync(IEnumerable<Comment> items)
        {
            foreach (var item in items)
            {
                item.User = await _users.GetByIdAsync(item.UserId);
            }

            return await base.MapAsync(items);
        }



        public async Task<Comment> AddAsync(string entityId, string userId, string content)
        {
            using (QueryFactory db = _queryFactory.Invoke())
            {
                string id = await db.Query("comments").InsertGetIdAsync<string>(new
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = userId,
                    Content = content,
                    Sent = DateTime.Now,
                    EntityId = entityId
                });

                return await GetByIdAsync(id);
            }
        }



        public async Task<IEntityCollection<Comment>> GetByEntityAsync(string entityId)
        {
            return await SelectAsync(query => query.Where("EntityId", entityId));
        }

        public async Task<Comment> GetByIdAsync(string id)
        {
            return await SelectSingleAsync(query => query.Where("comments.Id", id));
        }

        

        public async Task UpdateAsync(string id, string content)
        {
            using (QueryFactory db = _queryFactory.Invoke())
            {
                await db.Query("comments").Where("Id", id).UpdateAsync(new
                {
                    Content = content,
                    Edited = DateTime.Now
                });
            }
        }



        public async Task DeleteAsync(string id)
        {
            using (QueryFactory db = _queryFactory.Invoke())
            {
                await db.Query("comments").Where("Id", id).DeleteAsync();
            }
        }
    }
}
