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
using TobyMeehan.Com.Data.Extensions;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Sql;
using TobyMeehan.Sql.QueryBuilder;

namespace TobyMeehan.Com.Data.Sql
{
    public class UserTable : MultiMappingTable<User, Role, Transaction>
    {
        private readonly QueryFactory _factory;

        public UserTable(QueryFactory factory) : base(factory)
        {
            _factory = factory;
        }

        protected override ExecutableSqlQuery<User> GetSql()
        {
            return _factory.Executable<User>()
                .Select()
                .JoinUsers();
        }

        protected override User Map(User user, Role role, Transaction transaction)
        {
            return user.Map(role, transaction);
        }
    }
}
