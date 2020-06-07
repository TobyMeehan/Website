using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.CloudStorage;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Sql;

namespace TobyMeehan.Com.Data.Repositories
{
    public class SqlDownloadRepository : SqlRepository<Download>, IDownloadRepository
    {
        private readonly ISqlTable<Download> _table;
        private readonly ISqlTable<DownloadAuthor> _authorTable;
        private readonly ISqlTable<DownloadFile> _fileTable;
        private readonly ICloudStorage _cloudStorage;
        private readonly IConfiguration _configuration;

        public SqlDownloadRepository(ISqlTable<Download> table, ISqlTable<DownloadAuthor> authorTable, ISqlTable<DownloadFile> fileTable, ICloudStorage cloudStorage, IConfiguration configuration) : base(table)
        {
            _table = table;
            _authorTable = authorTable;
            _fileTable = fileTable;
            _cloudStorage = cloudStorage;
            _configuration = configuration;
        }

        public async Task<Download> AddAsync(string title, string shortDescription, string longDescription)
        {
            string id = Guid.NewGuid().ToString();

            await _table.InsertAsync(new
            {
                Id = id,
                Title = title,
                ShortDescription = shortDescription,
                LongDescription = longDescription
            });

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

        public async Task AddFileAsync(string id, string filename, Stream uploadStream)
        {
            string bucket = _configuration.GetSection("DownloadStorageBucket").Value;

            string fileId = Guid.NewGuid().ToString();

            string url = await _cloudStorage.UploadFileAsync(uploadStream, bucket, fileId);

            await _fileTable.InsertAsync(new
            {
                Id = fileId,
                DownloadId = id,
                Filename = filename,
                Url = url
            });
        }

        public async Task<IList<Download>> GetByAuthorAsync(string userId)
        {
            return (await _table.SelectByAsync<User>((d, u) => u.Id == userId)).ToList();
        }

        public Task UpdateAsync(Download download)
        {
            return _table.UpdateAsync(x => x.Id == $"{download.Id}", download);
        }
    }
}
