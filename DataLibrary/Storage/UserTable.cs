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

namespace TobyMeehan.Com.Data.Storage
{
    public class UserTable : SqlTable<User>
    {
        private readonly IDbConnection _connection;

        public UserTable(IDbConnection connection, IDbNameResolver nameResolver) : base(connection, nameResolver) {
            _connection = connection;
        }

        private string SelectWithJoin()
        {
            return $"SELECT u.*, r.Name, t.Sender, t.Description, t.Amount " +
                $"FROM `users` u " +
                $"LEFT JOIN `userroles` ur " +
                    $"ON ur.`UserId` = u.`Id` " +
                $"LEFT JOIN `roles` r " +
                    $"ON ur.`RoleId` = r.`Id` " +
                $"LEFT JOIN `transactions` t" +
                    $"ON t.`UserId` = u.`Id`";
        }

        private string SelectWithJoin(Expression<Predicate<User>> expression, out object parameters)
        {
            return $"{SelectWithJoin()}{new SqlQuery("users").Where(expression).AsSql(out parameters)}";
        }

        public override IEnumerable<User> Select()
        {
            return _connection.Query<User>(SelectWithJoin());
        }
        public override IEnumerable<User> Select(params string[] columns)
        {
            return Select();
        }


        public override Task<IEnumerable<User>> SelectAsync()
        {
            return _connection.QueryAsync<User>(SelectWithJoin());
        }
        public override Task<IEnumerable<User>> SelectAsync(params string[] columns)
        {
            return SelectAsync();
        }


        public override IEnumerable<User> SelectBy(Expression<Predicate<User>> expression)
        {
            return _connection.Query<User>(SelectWithJoin(expression, out object parameters), parameters);
        }
        public override IEnumerable<User> SelectBy(Expression<Predicate<User>> expression, params string[] columns)
        {
            return SelectBy(expression);
        }


        public override Task<IEnumerable<User>> SelectByAsync(Expression<Predicate<User>> expression)
        {
            return _connection.QueryAsync<User>(SelectWithJoin(expression, out object parameters), parameters);
        }
        public override Task<IEnumerable<User>> SelectByAsync(Expression<Predicate<User>> expression, params string[] columns)
        {
            return SelectByAsync(expression);
        }
    }
}
