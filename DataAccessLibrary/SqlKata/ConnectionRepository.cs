using SqlKata;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Data.SqlKata
{
    public class ConnectionRepository : RepositoryBase<IConnection, Connection>, IConnectionRepository
    {
        private readonly IIdGenerator _idGenerator;
        private readonly IUserRepository _users;
        private readonly IApplicationRepository _applications;
        private readonly string _table = "connections";

        public ConnectionRepository(QueryFactory queryFactory, IIdGenerator idGenerator, IUserRepository users, IApplicationRepository applications) : base(queryFactory)
        {
            _idGenerator = idGenerator;
            _users = users;
            _applications = applications;
        }

        protected override Query Query()
        {
            return base.Query()
                .From("connections");
        }

        protected override async Task<IReadOnlyList<IConnection>> MapAsync(IEnumerable<Connection> items)
        {
            var list = items.ToList();
            
            foreach (var item in list)
            {
                item.User = await _users.GetByIdAsync(item.UserId);
                item.Application = await _applications.GetByIdAsync(item.AppId);
            }

            return await base.MapAsync(list);
        }

        public Task<IConnection> GetByIdAsync(Id<IConnection> id)
        {
            return SelectSingleAsync(query => query.Where($"{_table}.Id", id));
        }

        public Task<IReadOnlyList<IConnection>> GetByUserAsync(Id<IUser> userId)
        {
            return SelectAsync(query => query.Where($"{_table}.UserId", userId.Value));
        }

        public Task<IReadOnlyList<IConnection>> GetByApplicationAsync(Id<IApplication> appId)
        {
            return SelectAsync(query => query.Where($"{_table}.AppId", appId.Value));
        }

        public async Task<IConnection> GetOrCreateAsync(Id<IUser> userId, Id<IApplication> appId)
        {
            var connection = await SelectSingleAsync(query => query.Where("UserId", userId).Where("AppId", appId));

            if (connection is not null)
            {
                return connection;
            }
            
            var id = _idGenerator.GenerateId<IConnection>();
            await QueryFactory.Query(_table).InsertAsync(new
            {
                Id = id.Value,
                UserId = userId.Value,
                AppId = appId.Value
            });

            return await GetByIdAsync(id);
        }

        public async Task DeleteByApplicationAsync(Id<IApplication> appId)
        {
            await QueryFactory.Query(_table).Where("AppId", appId.Value).DeleteAsync();
        }

        public async Task DeleteAsync(Id<IConnection> id)
        {
            await QueryFactory.Query(_table).Where("Id", id.Value).DeleteAsync();
        }
    }
}
