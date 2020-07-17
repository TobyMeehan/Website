using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Extensions;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Sql;
using TobyMeehan.Sql.QueryBuilder;

namespace TobyMeehan.Com.Data.Sql
{
    public class DownloadTable : MultiMappingTableBase<Download>
    {
        private readonly ISqlTable<Comment> _commentTable;

        public DownloadTable(Func<IDbConnection> connectionFactory, ISqlTable<Comment> commentTable) : base(connectionFactory)
        {
            _commentTable = commentTable;
        }

        protected override ISqlQuery<Download> GetQuery(Dictionary<string, Download> dictionary)
        {
            return base.GetQuery(dictionary)
                .JoinDownloads()
                .Map<DownloadFile, User, Role, Transaction>((download, file, user, role, transaction) =>
                {
                    if (!dictionary.TryGetValue(download.Id, out Download entry))
                    {
                        entry = download;
                        entry.Authors = new EntityCollection<User>();
                        entry.Files = new EntityCollection<DownloadFile>();

                        dictionary.Add(entry.Id, entry);
                    }

                    if (!entry.Authors.TryGetItem(user.Id, out User userEntry))
                    {
                        userEntry = user;
                        entry.Authors.Add(userEntry);
                    }

                    userEntry = userEntry.Map(role, transaction);

                    if (file != null)
                    {
                        if (!entry.Files.TryGetItem(file.Id, out DownloadFile fileEntry))
                        {
                            fileEntry = file;
                            entry.Files.Add(fileEntry);
                        }
                    }

                    return entry;
                });
        }

        private IEnumerable<Download> Join(IEnumerable<Download> downloads)
        {
            foreach (Download download in downloads)
            {
                download.Comments = _commentTable.SelectBy(c => c.EntityId == $"{download.Id}").Distinct().ToEntityCollection();
                yield return download;
            }
        }

        private async Task<IEnumerable<Download>> JoinAsync(IEnumerable<Download> downloads)
        {
            return (await Task.WhenAll(downloads.Select(async download =>
            {
                download.Comments = (await _commentTable.SelectByAsync(c => c.EntityId == $"{download.Id}")).Distinct().ToEntityCollection();
                return download;
            }))).ToList();
        }

        protected override IEnumerable<Download> Query()
        {
            return Join(base.Query());
        }

        protected override IEnumerable<Download> Query(Expression<Predicate<Download>> expression)
        {
            return Join(base.Query(expression));
        }

        protected override IEnumerable<Download> Query<TForeign>(Expression<Func<Download, TForeign, bool>> expression)
        {
            return Join(base.Query(expression));
        }

        protected override async Task<IEnumerable<Download>> QueryAsync()
        {
            return await JoinAsync(await base.QueryAsync());
        }

        protected override async Task<IEnumerable<Download>> QueryAsync(Expression<Predicate<Download>> expression)
        {
            return await JoinAsync(await base.QueryAsync(expression));
        }

        protected override async Task<IEnumerable<Download>> QueryAsync<TForeign>(Expression<Func<Download, TForeign, bool>> expression)
        {
            return await JoinAsync(await base.QueryAsync(expression));
        }
    }
}
