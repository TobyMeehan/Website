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
    public class ConnectionTable : MultiMappingTable<Connection, User, Application, User, Role, Transaction>
    {
        private readonly QueryFactory _factory;

        public ConnectionTable(QueryFactory factory) : base(factory)
        {
            _factory = factory;
        }

        protected override ExecutableSqlQuery<Connection> GetSql()
        {
            return _factory.Executable<Connection>()
                .Select()
                .JoinConnections();
        }

        protected override Connection Map(Connection connection, User connectionUser, Application app, User author, Role role, Transaction transaction)
        {
            connection.Application = connection.Application ?? app;
            connection.User = connection.User ?? connectionUser;

            connection.Application = app.Map(author, role, transaction);
            connection.User = connectionUser.Map(role, transaction);

            return connection;
        }
    }
}
