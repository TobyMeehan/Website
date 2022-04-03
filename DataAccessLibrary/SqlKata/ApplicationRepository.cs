using Microsoft.Extensions.Options;
using SqlKata;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.CloudStorage;
using TobyMeehan.Com.Data.Configuration;
using TobyMeehan.Com.Data.Extensions;

namespace TobyMeehan.Com.Data.SqlKata
{
    public class ApplicationRepository : RepositoryBase<IApplication, Application, NewApplication, EditApplication>, IApplicationRepository
    {
        private readonly ICloudStorage _storage;
        private readonly IDownloadRepository _downloads;
        private readonly IUserRepository _users;
        private readonly IConnectionRepository _connections;
        private readonly CloudStorageOptions _options;

        public ApplicationRepository(QueryFactory queryFactory, IIdGenerator idGenerator, ICloudStorage storage, IOptions<CloudStorageOptions> options, IDownloadRepository downloads, IUserRepository users, IConnectionRepository connections) 
            : base (queryFactory, idGenerator, "applications")
        {
            _storage = storage;
            _downloads = downloads;
            _users = users;
            _connections = connections;
            _options = options.Value;
        }

        protected override Query Query()
        {
            return base.Query()
                .From(Table)
                .OrderBy("Name");
        }

        protected override async Task<IReadOnlyList<IApplication>> MapAsync(IEnumerable<Application> items)
        {
            var list = items.ToList();
            
            foreach (var item in list)
            {
                item.Author = await _users.GetByIdAsync(item.UserId);
                item.Download = await _downloads.GetByIdAsync(item.DownloadId);
            }

            return await base.MapAsync(list);
        }

        public Task<IReadOnlyList<IApplication>> GetByUserAsync(Id<IUser> userId)
        {
            return SelectAsync(query => query.Where($"{Table}.UserId", userId.Value));
        }

        public override async Task DeleteAsync(Id<IApplication> id)
        {
            await _connections.DeleteByApplicationAsync(id);
            
            await base.DeleteAsync(id);
        }

        public async Task<string> AddIconAsync(Id<IApplication> id, Action<FileUpload> configureFile,
            CancellationToken cancellationToken = default)
        {
            var upload = configureFile.Apply(new FileUpload());
            
            var file = await _storage.UploadFileAsync(upload.UploadStream, _options.AppIconStorageBucket, id.Value, upload.Filename,
                upload.ContentType, cancellationToken);

            await QueryFactory.Query(Table).Where("Id", id.Value).UpdateAsync(new {IconUrl = file.MediaLink}, cancellationToken: cancellationToken);

            return file.MediaLink;
        }

        public async Task RemoveIconAsync(Id<IApplication> id)
        {
            await _storage.DeleteFileAsync(_options.DownloadStorageBucket, id.Value);
            
            await QueryFactory.Query(Table).Where("Id", id.Value).UpdateAsync(new {IconUrl = null as string});
        }

        public async Task<bool> ValidateAsync(Id<IApplication> id, string secret, string redirectUri, bool ignoreSecret)
        {
            var application = await GetByIdAsync(id);

            if (application == null)
                return false;

            return
                (ignoreSecret || secret != null && application.Secret == secret) &&
                new Uri(application.RedirectUri) == new Uri(redirectUri);
        }
    }
}
