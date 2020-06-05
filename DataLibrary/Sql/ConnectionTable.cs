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
    public class ConnectionTable : MultiMappingTableBase<Connection>
    {
        public ConnectionTable(Func<IDbConnection> connectionFactory) : base(connectionFactory)
        {

        }

        protected override ISqlQuery<Connection> GetQuery(Dictionary<string, Connection> dictionary)
        {
            return base.GetQuery(dictionary)
                .JoinConnections()
                .Map<User, Application, User, Role, Transaction>((connection, user, app, author, role, transaction) =>
                {
                    if (!dictionary.TryGetValue(connection.Id, out Connection entry))
                    {
                        entry = connection;
                    }

                    entry.User = entry.User ?? user;

                    entry.Application = entry.Application ?? app;
                    entry.Application = entry.Application.Map(author, role, transaction);

                    return entry;
                });
        }
    }
}
