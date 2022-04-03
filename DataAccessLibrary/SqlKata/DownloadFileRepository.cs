using Microsoft.Extensions.Options;
using SqlKata;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.CloudStorage;
using TobyMeehan.Com.Data.Configuration;
using TobyMeehan.Com.Data.Extensions;
using TobyMeehan.Com.Data.Upload;

namespace TobyMeehan.Com.Data.SqlKata
{
    public class DownloadFileRepository : RepositoryBase<IDownloadFile, DownloadFile, NewDownloadFile, EditDownloadFile>, IDownloadFileRepository
    {
        private readonly ICloudStorage _storage;
        private readonly CloudStorageOptions _options;

        public DownloadFileRepository(QueryFactory queryFactory, ICloudStorage storage, IIdGenerator idGenerator, IOptions<CloudStorageOptions> options) : base (queryFactory, idGenerator, "downloadfiles")
        {
            _storage = storage;
            _options = options.Value;
        }

        protected override Query Query()
        {
            return base.Query()
                .From(Table)
                .OrderBy("Filename");
        }

        protected override async Task<IReadOnlyList<IDownloadFile>> MapAsync(IEnumerable<DownloadFile> items)
        {
            var files = items.ToList();
            
            foreach (var file in files)
            {
                file.InnerFile = await _storage.GetFileAsync(_options.DownloadStorageBucket, file.Id.Value);
            }
            
            return await base.MapAsync(files);
        }

        public async Task<IDownloadFile> AddAsync(Id<IDownload> downloadId, Action<FileUpload> configureFile, CancellationToken cancellationToken = default, IProgress<IUploadProgress> progress = null)
        {
            var upload = configureFile.Apply(new FileUpload());
            var bucket = _options.DownloadStorageBucket;
            var id = GenerateId();

            var file = await _storage.UploadFileAsync(upload.UploadStream, bucket, id.Value, upload.Filename, upload.ContentType,
                cancellationToken, progress);
            
            return await base.AddAsync(f =>
            {
                f.DownloadId = downloadId;
                f.Filename = file.Filename;
            }, id);
        }
        
        public async Task<IReadOnlyList<IDownloadFile>> GetByDownloadAsync(Id<IDownload> downloadId)
        {
            return await SelectAsync(query => query.Where($"{Table}.DownloadId", downloadId.Value));
        }

        public async Task<IReadOnlyList<IDownloadFile>> GetByDownloadAndFilenameAsync(Id<IDownload> downloadId, string filename)
        {
            return await SelectAsync(query =>
                query.Where($"{Table}.DownloadId", downloadId.Value).Where($"{Table}.Filename", filename));
        }

        public async Task DownloadAsync(Id<IDownloadFile> id, Stream stream)
        {
            var bucket = _options.DownloadStorageBucket;

            await _storage.DownloadFileAsync(bucket, id.Value, stream);
        }
    }
}
