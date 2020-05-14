using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Sql;
using TobyMeehan.Sql.QueryBuilder;

namespace TobyMeehan.Com.Data.Database
{
    public class ConnectionTable : SqlTable<Connection>
    {
        private readonly IDbConnection _connection;

        public ConnectionTable(IDbConnection connection, IDbNameResolver nameResolver) : base(connection, nameResolver)
        {
            _connection = connection;
        }

        internal static string GetJoinQuery(string connection, string connectionUser, string app, string applicationAuthor, string role, string transaction)
        {
            return
                $"INNER JOIN `users` {connectionUser} " +
                    $"ON {connection}.`UserId` = {connectionUser}.`Id` " + 
                    UserTable.GetJoinQuery(connectionUser, role, transaction) +
                $"INNER JOIN `applications` {app} " +
                    $"ON {app}.`Id` = {connection}.`AppId` " +
                    ApplicationTable.GetJoinQuery(app, applicationAuthor, role, transaction);
        }

        private string GetSelectQuery()
        {
            return
                $"SELECT c.*, " +
                ApplicationTable.GetColumns("a") +
                UserTable.GetColumns("u", "r", "t") +
                "FROM `connections` c " +
                GetJoinQuery("c", "cu", "a", "au", "r", "t");
        }

        private string GetSelectQuery(Expression<Predicate<Connection>> expression, out object parameters)
        {
            return
                GetSelectQuery() +
                new SqlQuery("connections").Where(expression).AsSql(out parameters);
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
            _connection.Query<Connection, User, Application, User, Role, Transaction, Connection>(GetSelectQuery(), Map);

        private IEnumerable<Connection> Query(Expression<Predicate<Connection>> expression) =>
            _connection.Query<Connection, User, Application, User, Role, Transaction, Connection>(GetSelectQuery(expression, out object parameters), Map, parameters);

        private Task<IEnumerable<Connection>> QueryAsync() =>
            _connection.QueryAsync<Connection, User, Application, User, Role, Transaction, Connection>(GetSelectQuery(), Map);

        private Task<IEnumerable<Connection>> QueryAsync(Expression<Predicate<Connection>> expression) =>
            _connection.QueryAsync<Connection, User, Application, User, Role, Transaction, Connection>(GetSelectQuery(expression, out object parameters), Map, parameters);

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
