using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Sql;
using TobyMeehan.Sql.QueryBuilder;

namespace TobyMeehan.Com.Data.Sql
{
    public class ConnectionTable : MultiMappingTableBase<Connection>
    {
        private readonly ISqlTable<Application> _applicationTable;
        private readonly ISqlTable<User> _userTable;

        public ConnectionTable(Func<IDbConnection> connectionFactory, ISqlTable<Application> applicationTable, ISqlTable<User> userTable) : base(connectionFactory)
        {
            _applicationTable = applicationTable;
            _userTable = userTable;
        }

        protected override ISqlQuery<Connection> GetQuery(Dictionary<string, Connection> dictionary)
        {
            return new SqlQuery<Connection>()
                .Select()
                .InnerJoin<Application>((c, a) => c.AppId == a.Id)
                .InnerJoin<User>((c, u) => c.UserId == u.Id)
                .Map<Application, User>((connection, app, user) =>
                {
                    if (!dictionary.TryGetValue(connection.Id, out Connection entry))
                    {
                        entry = connection;
                    }

                    entry.User = entry.User ?? user;
                    entry.Application = entry.Application ?? app;

                    return entry;
                });
        }

        private IEnumerable<Connection> Join(IEnumerable<Connection> connections)
        {
            foreach (Connection connection in connections)
            {
                connection.User = _userTable.SelectBy(u => u.Id == $"{connection.UserId}").Distinct().Single();
                connection.Application = _applicationTable.SelectBy(a => a.Id == $"{connection.AppId}").Distinct().Single();
                yield return connection;
            }
        }

        private async Task<IEnumerable<Connection>> JoinAsync(IEnumerable<Connection> connections)
        {
            return (await Task.WhenAll(connections.Select(async connection =>
            {
                var users = await _userTable.SelectByAsync(u => u.Id == $"{connection.UserId}");
                var users1 = users.Distinct();
                connection.User = (await _userTable.SelectByAsync(u => u.Id == $"{connection.UserId}")).Distinct().Single();
                var app = await _applicationTable.SelectByAsync(a => a.Id == $"{connection.AppId}");
                var app1 = app.Distinct();
                connection.Application = (await _applicationTable.SelectByAsync(a => a.Id == $"{connection.AppId}")).Distinct().Single();
                return connection;
            }))).ToList();
        }

        protected override IEnumerable<Connection> Query()
        {
            return Join(base.Query());
        }

        protected override IEnumerable<Connection> Query(Expression<Predicate<Connection>> expression)
        {
            return Join(base.Query(expression));
        }

        protected override IEnumerable<Connection> Query<TForeign>(Expression<Func<Connection, TForeign, bool>> expression)
        {
            return Join(base.Query(expression));
        }

        protected override async Task<IEnumerable<Connection>> QueryAsync()
        {
            return await JoinAsync(await base.QueryAsync());
        }

        protected override async Task<IEnumerable<Connection>> QueryAsync(Expression<Predicate<Connection>> expression)
        {
            return await JoinAsync(await base.QueryAsync(expression));
        }

        protected override async Task<IEnumerable<Connection>> QueryAsync<TForeign>(Expression<Func<Connection, TForeign, bool>> expression)
        {
            return await JoinAsync(await base.QueryAsync(expression));
        }
    }
}
