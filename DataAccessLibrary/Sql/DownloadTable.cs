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
        public DownloadTable(Func<IDbConnection> connectionFactory) : base(connectionFactory)
        { 

        }

        protected override ISqlQuery<Download> GetQuery(Dictionary<string, Download> dictionary)
        {
            return base.GetQuery(dictionary)
                .JoinDownloads()
                .Map<DownloadFile, User, Role>((download, file, user, role) =>
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

                    userEntry = userEntry.Map(role);

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
    }
}
