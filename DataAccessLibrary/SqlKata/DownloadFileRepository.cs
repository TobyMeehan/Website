using Microsoft.Extensions.Options;
using SqlKata;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.CloudStorage;
using TobyMeehan.Com.Data.Collections;
using TobyMeehan.Com.Data.Configuration;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories;
using TobyMeehan.Com.Data.Upload;

namespace TobyMeehan.Com.Data.SqlKata
{
    public class DownloadFileRepository : RepositoryBase<DownloadFile>, IDownloadFileRepository
    {
        private readonly Func<QueryFactory> _queryFactory;
        private readonly ICloudStorage _storage;
        private readonly CloudStorageOptions _options;

        public DownloadFileRepository(Func<QueryFactory> queryFactory, ICloudStorage storage, IOptions<CloudStorageOptions> options) : base (queryFactory)
        {
            _queryFactory = queryFactory;
            _storage = storage;
            _options = options.Value;
        }

        protected override Query Query()
        {
            return base.Query()
                .From("downloadfiles");
        }

        public async Task<DownloadFile> AddAsync(string downloadId, string filename, Stream uploadStream, CancellationToken cancellationToken = default, IProgress<IUploadProgress> progress = null)
        {
            string bucket = _options.DownloadStorageBucket;
            string id = Guid.NewGuid().ToString();

            CloudFile cf = await _storage.UploadFileAsync(uploadStream, bucket, id, filename, MediaTypeNames.Application.Octet, cancellationToken, progress);

            using (QueryFactory db = _queryFactory.Invoke())
            {
                await db.Query("downloadfiles").InsertAsync(new
                {
                    Id = id,
                    DownloadId = downloadId,
                    Filename = filename,
                    Url = cf.DownloadLink
                });

                return await GetByIdAsync(id);
            }
        }



        public async Task<IEntityCollection<DownloadFile>> GetAsync()
        {
            return await SelectAsync();
        }

        public async Task<IEntityCollection<DownloadFile>> GetByDownloadAndFilenameAsync(string downloadId, string filename)
        {
            return await SelectAsync(query => query.Where("DownloadId", downloadId).Where("Filename", filename));
        }

        public async Task<IEntityCollection<DownloadFile>> GetByDownloadAsync(string downloadId)
        {
            return await SelectAsync(query => query.Where("DownloadId", downloadId));
        }

        public async Task<IEntityCollection<DownloadFile>> GetByFilenameAsync(string filename)
        {
            return await SelectAsync(query => query.Where("Filename", filename));
        }

        public async Task<DownloadFile> GetByIdAsync(string id)
        {
            return await SelectSingleAsync(query => query.Where("Id", id));
        }



        public async Task DeleteAsync(string id)
        {
            using (QueryFactory db = _queryFactory.Invoke())
            {
                await db.Query("downloadfiles").Where("Id", id).DeleteAsync();
            }
        }



        public Task DownloadAsync(string id, Stream stream)
        {
            string bucket = _options.DownloadStorageBucket;

            return _storage.DownloadFileAsync(bucket, id, stream);
        }
    }
}
