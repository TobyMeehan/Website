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
    public class ApplicationTable : SqlTable<Application>
    {
        private readonly IDbConnection _connection;

        public ApplicationTable(IDbConnection connection, IDbNameResolver nameResolver) : base(connection, nameResolver)
        {
            _connection = connection;
        }

        private string GetSelectQuery()
        {
            return 
                $"SELECT a.*, u.Username, u.Email, u.Balance, r.Name, t.Sender, t.Description, t.Amount " +
                $"FROM `applications` a " +
                $"INNER JOIN `users` u " +
                    $"ON u.`Id` = a.`UserId`" +
                UserTable.GetJoinQuery();
        }

        private string GetSelectQuery(Expression<Predicate<Application>> expression, out object parameters)
        {
            return $"{GetSelectQuery()}{new SqlQuery("users").Where(expression).AsSql(out parameters)}";
        }

        private Application Map(Application app, User user, Role role, Transaction transaction)
        {
            app.Author = app.Author ?? user;

            app.Author = UserTable.Map(app.Author, role, transaction);

            return app;
        }

        private IEnumerable<Application> Query()
        {
            return _connection.Query<Application, User, Role, Transaction, Application>(GetSelectQuery(), Map);
        }

        private Task<IEnumerable<Application>> QueryAsync()
        {
            return _connection.QueryAsync<Application, User, Role, Transaction, Application>(GetSelectQuery(), Map);
        }

        private IEnumerable<Application> Query(Expression<Predicate<Application>> expression)
        {
            return _connection.Query<Application, User, Role, Transaction, Application>(GetSelectQuery(expression, out object parameters), Map, parameters);
        }

        private Task<IEnumerable<Application>> QueryAsync(Expression<Predicate<Application>> expression)
        {
            return _connection.QueryAsync<Application, User, Role, Transaction, Application>(GetSelectQuery(expression, out object parameters), Map, parameters);
        }


        public override IEnumerable<Application> Select()
        {
            return Query();
        }
        public override IEnumerable<Application> Select(params string[] columns)
        {
            return Select();
        }


        public override Task<IEnumerable<Application>> SelectAsync()
        {
            return QueryAsync();
        }
        public override Task<IEnumerable<Application>> SelectAsync(params string[] columns)
        {
            return SelectAsync();
        }


        public override IEnumerable<Application> SelectBy(Expression<Predicate<Application>> expression)
        {
            return Query(expression);
        }
        public override IEnumerable<Application> SelectBy(Expression<Predicate<Application>> expression, params string[] columns)
        {
            return SelectBy(expression);
        }


        public override Task<IEnumerable<Application>> SelectByAsync(Expression<Predicate<Application>> expression)
        {
            return QueryAsync(expression);
        }
        public override Task<IEnumerable<Application>> SelectByAsync(Expression<Predicate<Application>> expression, params string[] columns)
        {
            return SelectByAsync(expression);
        }
    }
}
