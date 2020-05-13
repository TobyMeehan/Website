using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Sql;
using TobyMeehan.Sql.QueryBuilder;

namespace TobyMeehan.Com.Data.Database
{
    public class UserTable : SqlTable<User>
    {
        private readonly IDbConnection _connection;

        public UserTable(IDbConnection connection, IDbNameResolver nameResolver) : base(connection, nameResolver)
        {
            _connection = connection;
        }

        internal static string GetJoinQuery()
        {
            return 
                $"LEFT JOIN `userroles` ur " +
                    $"ON ur.`UserId` = u.`Id` " +
                $"LEFT JOIN `roles` r " +
                    $"ON ur.`RoleId` = r.`Id` " +
                $"LEFT JOIN `transactions` t" +
                    $"ON t.`UserId` = u.`Id`";
        }

        private string GetSelectQuery()
        {
            return 
                $"SELECT u.*, r.Name, t.Sender, t.Description, t.Amount " +
                $"FROM `users` u " +
                GetJoinQuery();
        }

        private string GetSelectQuery(Expression<Predicate<User>> expression, out object parameters)
        {
            return $"{GetSelectQuery()}{new SqlQuery("users").Where(expression).AsSql(out parameters)}";
        }

        internal static User Map(User user, Role role, Transaction transaction)
        {
            user.Roles = user.Roles ?? new List<Role>();
            user.Transactions = user.Transactions ?? new List<Transaction>();

            user.Roles.Add(role);
            user.Transactions.Add(transaction);

            return user;
        }

        private IEnumerable<User> Query()
        {
            return _connection.Query<User, Role, Transaction, User>(GetSelectQuery(), Map);
        }

        private Task<IEnumerable<User>> QueryAsync()
        {
            return _connection.QueryAsync<User, Role, Transaction, User>(GetSelectQuery(), Map);
        }

        private IEnumerable<User> Query(Expression<Predicate<User>> expression)
        {
            return _connection.Query<User, Role, Transaction, User>(GetSelectQuery(expression, out object parameters), Map, parameters);
        }

        private Task<IEnumerable<User>> QueryAsync(Expression<Predicate<User>> expression)
        {
            return _connection.QueryAsync<User, Role, Transaction, User>(GetSelectQuery(expression, out object parameters), Map, parameters);
        }


        public override IEnumerable<User> Select()
        {
            return Query();
        }
        public override IEnumerable<User> Select(params string[] columns)
        {
            return Select();
        }


        public override Task<IEnumerable<User>> SelectAsync()
        {
            return QueryAsync();
        }
        public override Task<IEnumerable<User>> SelectAsync(params string[] columns)
        {
            return SelectAsync();
        }


        public override IEnumerable<User> SelectBy(Expression<Predicate<User>> expression)
        {
            return Query(expression);
        }
        public override IEnumerable<User> SelectBy(Expression<Predicate<User>> expression, params string[] columns)
        {
            return SelectBy(expression);
        }


        public override Task<IEnumerable<User>> SelectByAsync(Expression<Predicate<User>> expression)
        {
            return QueryAsync(expression);
        }
        public override Task<IEnumerable<User>> SelectByAsync(Expression<Predicate<User>> expression, params string[] columns)
        {
            return SelectByAsync(expression);
        }
    }
}
