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
    public class ConnectionRepository : RepositoryBase<Connection>, IConnectionRepository
    {
        private readonly Func<QueryFactory> _queryFactory;
        private readonly IUserRepository _users;
        private readonly IApplicationRepository _applications;

        public ConnectionRepository(Func<QueryFactory> queryFactory, IUserRepository users, IApplicationRepository applications) : base(queryFactory)
        {
            _queryFactory = queryFactory;
            _users = users;
            _applications = applications;
        }

        protected override Query Query()
        {
            return base.Query()
                .From("connections");
        }

        protected override async Task<IEntityCollection<Connection>> MapAsync(IEnumerable<Connection> items)
        {
            foreach (var item in items)
            {
                item.User = await _users.GetByIdAsync(item.UserId);
                item.Application = await _applications.GetByIdAsync(item.AppId);
            }

            return await base.MapAsync(items);
        }



        public async Task<IEntityCollection<Connection>> GetAsync()
        {
            return await SelectAsync();
        }

        public async Task<IEntityCollection<Connection>> GetByApplicationAsync(string appId)
        {
            return await SelectAsync(query => query.Where("AppId", appId));
        }

        public async Task<Connection> GetByIdAsync(string id)
        {
            return await SelectSingleAsync(query => query.Where("Id", id));
        }

        public async Task<IEntityCollection<Connection>> GetByUserAsync(string userId)
        {
            return await SelectAsync(query => query.Where("UserId", userId));
        }

        public async Task<Connection> GetOrCreateAsync(string userId, string appId)
        {
            Connection connection = await SelectSingleAsync(query => query.Where("UserId", userId).Where("AppId", appId));

            if (connection == null)
            {
                using (QueryFactory db = _queryFactory.Invoke())
                {
                    string id = await db.Query("connections").InsertGetIdAsync<string>(new
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = userId,
                        AppId = appId
                    });

                    connection = await GetByIdAsync(id);
                }
            }

            return connection;
        }



        public async Task DeleteAsync(string id)
        {
            using (QueryFactory db = _queryFactory.Invoke())
            {
                await db.Query("connections").Where("Id", id).DeleteAsync();
            }
        }
    }
}
