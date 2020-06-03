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
    public class DownloadTable : MultiMappingTable<Download, User, Role, Transaction, DownloadFile>
    {
        private readonly QueryFactory _queryFactory;

        public DownloadTable(QueryFactory queryFactory) : base(queryFactory)
        {
            _queryFactory = queryFactory;
        }

        protected override ExecutableSqlQuery<Download> GetSql()
        {
            return _queryFactory.Executable<Download>()
                .Select()
                .JoinDownloads();
        }

        protected override Download Map(Download download, User user, Role role, Transaction transaction, DownloadFile file)
        {
            download.Authors = download.Authors ?? new List<User>();
            download.Files = download.Files ?? new List<DownloadFile>();

            download.Authors.Add(user.Map(role, transaction));
            download.Files.Add(file);

            return download;
        }
    }
}
