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
    public class DownloadTable : SqlTable<Download>
    {
        private readonly IDbConnection _connection;

        public DownloadTable(IDbConnection connection) : base(connection)
        {
            _connection = connection;
        }

        private SqlQuery<Download> GetSelectQuery()
        {
            return new SqlQuery<Download>()
                .Select()
                .JoinDownloads();
        }

        private SqlQuery<Download> GetSelectQuery(Expression<Predicate<Download>> expression)
        {
            return GetSelectQuery()
                .Where(expression);
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
            _connection.Query<Download, User, Role, Transaction, DownloadFile, Download>(GetSelectQuery().AsSql(), Map).DistinctEntities();

        private IEnumerable<Download> Query(Expression<Predicate<Download>> expression) =>
            _connection.Query<Download, User, Role, Transaction, DownloadFile, Download>(GetSelectQuery(expression).AsSql(out object parameters), Map, parameters).DistinctEntities();

        private async Task<IEnumerable<Download>> QueryAsync() =>
            (await _connection.QueryAsync<Download, User, Role, Transaction, DownloadFile, Download>(GetSelectQuery().AsSql(), Map)).DistinctEntities();

        private async Task<IEnumerable<Download>> QueryAsync(Expression<Predicate<Download>> expression) =>
            (await _connection.QueryAsync<Download, User, Role, Transaction, DownloadFile, Download>(GetSelectQuery(expression).AsSql(out object parameters), Map, parameters)).DistinctEntities();

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
