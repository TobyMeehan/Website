using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Sql;

namespace TobyMeehan.Com.Data.Database
{
    public class ConnectionTable : SqlTable<Connection>
    {
        private readonly IDbConnection _connection;

        public ConnectionTable(IDbConnection connection, IDbNameResolver nameResolver) : base(connection, nameResolver)
        {
            _connection = connection;
        }

        internal static string GetJoinQuery(string connection, string app, string user, string role, string transaction)
        {
            return
                $"INNER JOIN `applications` {app} " +
                $"ON {app}.`Id` = {connection}.`AppId` " +
                ApplicationTable.GetJoinQuery(app, user, role, transaction);
        }
    }
}
