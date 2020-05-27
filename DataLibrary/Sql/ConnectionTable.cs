using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Extensions;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Sql;
using TobyMeehan.Sql.QueryBuilder;

namespace TobyMeehan.Com.Data.Sql
{
    public class ConnectionTable : SqlTable<Connection>
    {
        private readonly IDbConnection _connection;

        public ConnectionTable(IDbConnection connection) : base(connection)
        {
            _connection = connection;
        }

        private SqlQuery<Connection> GetSelectQuery()
        {
            return new SqlQuery<Connection>()
                .Select()
                .JoinConnections();
        }

        private SqlQuery<Connection> GetSelectQuery(Expression<Predicate<Connection>> expression)
        {
            return
                GetSelectQuery()
                .Where(expression);
        }

        private Connection Map(Connection connection, User connectionUser, Application app, User author, Role role, Transaction transaction)
        {
            connection.Application = connection.Application ?? app;
            connection.User = connection.User ?? connectionUser;

            connection.Application = ApplicationTable.Map(app, author, role, transaction);
            connection.User = UserTable.Map(connectionUser, role, transaction);

            return connection;
        }

        private IEnumerable<Connection> Query() => 
            _connection.Query<Connection, User, Application, User, Role, Transaction, Connection>(GetSelectQuery().AsSql(), Map);

        private IEnumerable<Connection> Query(Expression<Predicate<Connection>> expression) =>
            _connection.Query<Connection, User, Application, User, Role, Transaction, Connection>(GetSelectQuery(expression).AsSql(out object parameters), Map, parameters);

        private Task<IEnumerable<Connection>> QueryAsync() =>
            _connection.QueryAsync<Connection, User, Application, User, Role, Transaction, Connection>(GetSelectQuery().AsSql(), Map);

        private Task<IEnumerable<Connection>> QueryAsync(Expression<Predicate<Connection>> expression) =>
            _connection.QueryAsync<Connection, User, Application, User, Role, Transaction, Connection>(GetSelectQuery(expression).AsSql(out object parameters), Map, parameters);

        public override IEnumerable<Connection> Select()
        {
            return Query();
        }

        public override IEnumerable<Connection> Select(params string[] columns) => Select();

        public override Task<IEnumerable<Connection>> SelectAsync()
        {
            return QueryAsync();
        }
        public override Task<IEnumerable<Connection>> SelectAsync(params string[] columns) => SelectAsync();

        public override IEnumerable<Connection> SelectBy(Expression<Predicate<Connection>> expression)
        {
            return Query(expression);
        }
        public override IEnumerable<Connection> SelectBy(Expression<Predicate<Connection>> expression, params string[] columns) => SelectBy(expression);

        public override Task<IEnumerable<Connection>> SelectByAsync(Expression<Predicate<Connection>> expression)
        {
            return QueryAsync(expression);
        }

        public override Task<IEnumerable<Connection>> SelectByAsync(Expression<Predicate<Connection>> expression, params string[] columns) => SelectByAsync(expression);
    }
}
