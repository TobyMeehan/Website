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
    public class DownloadTable : SqlTable<Download>
    {
        private readonly IDbConnection _connection;

        public DownloadTable(IDbConnection connection, IDbNameResolver nameResolver) : base(connection, nameResolver)
        {
            _connection = connection;
        }

        internal static string GetJoinQuery(string download, string file, string user, string role, string transaction)
        {
            return
                $"INNER JOIN `downloadauthors` {download}{user} " +
                    $"ON {download}{user}.`DownloadId` = {download}.`Id` " +
                    $"INNER JOIN `users` {user} " +
                        $"ON {user}.`Id` = {download}{user}.`UserId` " +
                        UserTable.GetJoinQuery(user, role, transaction) +
                $"LEFT JOIN `downloadfiles` {file} " +
                    $"ON {file}.`DownloadId` = {download}.`Id` ";
        }

        private string GetSelectQuery()
        {
            return
                $"SELECT d.*, {UserTable.GetColumns("u", "r", "t")} " +
                $"FROM `downloads` d " +
                GetJoinQuery("d", "f", "u", "r", "t");
        }

        private string GetSelectQuery(Expression<Predicate<Download>> expression, out object parameters)
        {
            return $"{GetSelectQuery()}{new SqlQuery("downloads").Where(expression).AsSql(out parameters)}";
        }

        private Download Map(Download download, User user, Role role, Transaction transaction, DownloadFile file)
        {
            download.Authors = download.Authors ?? new List<User>();
            download.Files = download.Files ?? new List<DownloadFile>();

            download.Authors.Add(UserTable.Map(user, role, transaction));
            download.Files.Add(file);

            return download;
        }

        private IEnumerable<Download> Query() =>
            _connection.Query<Download, User, Role, Transaction, DownloadFile, Download>(GetSelectQuery(), Map);

        private IEnumerable<Download> Query(Expression<Predicate<Download>> expression) =>
            _connection.Query<Download, User, Role, Transaction, DownloadFile, Download>(GetSelectQuery(expression, out object parameters), Map, parameters);

        private Task<IEnumerable<Download>> QueryAsync() =>
            _connection.QueryAsync<Download, User, Role, Transaction, DownloadFile, Download>(GetSelectQuery(), Map);

        private Task<IEnumerable<Download>> QueryAsync(Expression<Predicate<Download>> expression) =>
            _connection.QueryAsync<Download, User, Role, Transaction, DownloadFile, Download>(GetSelectQuery(expression, out object parameters), Map, parameters);

        public override IEnumerable<Download> Select()
        {
            return Query();
        }
        public override IEnumerable<Download> Select(params string[] columns) => Select();

        public override Task<IEnumerable<Download>> SelectAsync()
        {
            return QueryAsync();
        }
        public override Task<IEnumerable<Download>> SelectAsync(params string[] columns) => SelectAsync();

        public override IEnumerable<Download> SelectBy(Expression<Predicate<Download>> expression)
        {
            return Query(expression);
        }
        public override IEnumerable<Download> SelectBy(Expression<Predicate<Download>> expression, params string[] columns) => SelectBy(expression);

        public override Task<IEnumerable<Download>> SelectByAsync(Expression<Predicate<Download>> expression)
        {
            return QueryAsync(expression);
        }
        public override Task<IEnumerable<Download>> SelectByAsync(Expression<Predicate<Download>> expression, params string[] columns) => SelectByAsync(expression);
    }
}
