using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.CloudStorage;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Security;
using TobyMeehan.Com.Data.Upload;
using TobyMeehan.Sql;

namespace TobyMeehan.Com.Data.Repositories
{
    public class SqlDownloadRepository : SqlRepository<Download>, IDownloadRepository
    {
        private readonly ISqlTable<Download> _table;
        private readonly ISqlTable<DownloadAuthor> _authorTable;

        public SqlDownloadRepository(ISqlTable<Download> table, ISqlTable<DownloadAuthor> authorTable) : base(table)
        {
            _table = table;
            _authorTable = authorTable;
        }

        public async Task<Download> AddAsync(string title, string shortDescription, string longDescription, string userId)
        {
            string id = RandomString.GeneratePseudo();

            await _table.InsertAsync(new
            {
                Id = id,
                Title = title,
                ShortDescription = shortDescription,
                LongDescription = longDescription
            });

            await AddAuthorAsync(id, userId);

            return await GetByIdAsync(id);
        }

        public Task AddAuthorAsync(string id, string userId)
        {
            return _authorTable.InsertAsync(new
            {
                DownloadId = id,
                UserId = userId
            });
        }

        public async Task<IList<Download>> GetByAuthorAsync(string userId)
        {
            return (await _table.SelectByAsync<User>((d, u) => u.Id == userId)).ToList();
        }

        public Task RemoveAuthorAsync(string id, string userId)
        {
            return _authorTable.DeleteAsync(x => x.DownloadId == id && x.UserId == userId);
        }

        public Task UpdateAsync(Download download)
        {
            return _table.UpdateAsync(x => x.Id == $"{download.Id}", new
            {
                download.Title,
                download.ShortDescription,
                download.LongDescription,
                download.Verified
            });
        }
    }
}
