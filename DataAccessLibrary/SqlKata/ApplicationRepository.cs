using Microsoft.Extensions.Options;
using SqlKata;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.CloudStorage;
using TobyMeehan.Com.Data.Collections;
using TobyMeehan.Com.Data.Configuration;
using TobyMeehan.Com.Data.Extensions;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories;

namespace TobyMeehan.Com.Data.SqlKata
{
    public class ApplicationRepository : RepositoryBase<Application>, IApplicationRepository
    {
        private readonly Func<QueryFactory> _queryFactory;
        private readonly ICloudStorage _storage;
        private readonly IDownloadRepository _downloads;
        private readonly IUserRepository _users;
        private readonly CloudStorageOptions _options;

        public ApplicationRepository(Func<QueryFactory> queryFactory, ICloudStorage storage, IOptions<CloudStorageOptions> options, IDownloadRepository downloads, IUserRepository users) : base (queryFactory)
        {
            _queryFactory = queryFactory;
            _storage = storage;
            _downloads = downloads;
            _users = users;
            _options = options.Value;

        }

        protected override Query Query()
        {
            return base.Query()
                .From("applications")
                .OrderBy("Name");
        }

        protected override async Task<IEntityCollection<Application>> MapAsync(IEnumerable<Application> items)
        {
            foreach (var item in items)
            {
                item.Author = await _users.GetByIdAsync(item.UserId);
                item.Download = await _downloads.GetByIdAsync(item.DownloadId);
            }

            return await base.MapAsync(items);
        }



        public async Task<Application> AddAsync(string userId, string name, string redirectUri, bool secret)
        {
            string id = Guid.NewGuid().ToString();

            using (QueryFactory db = _queryFactory.Invoke())
            {
                await db.Query("applications").InsertAsync(new
                {
                    Id = id,
                    UserId = userId,
                    Name = name,
                    RedirectUri = redirectUri,
                    Secret = secret ? Guid.NewGuid().ToToken() : null
                });
            }

            return await GetByIdAsync(id);
        }



        public async Task<IEntityCollection<Application>> GetAsync()
        {
            return await SelectAsync();
        }

        public async Task<Application> GetByIdAsync(string id)
        {
            return await SelectSingleAsync(query => query.Where("applications.Id", id));
        }

        public async Task<IEntityCollection<Application>> GetByNameAsync(string name)
        {
            return await SelectAsync(query => query.Where("Name", name));
        }

        public async Task<Application> GetByUserAndNameAsync(string userId, string name)
        {
            return await SelectSingleAsync(query => query.Where("UserId", userId).Where("Name", name));
        }

        public async Task<IEntityCollection<Application>> GetByUserAsync(string userId)
        {
            return await SelectAsync(query => query.Where("UserId", userId));
        }



        public async Task UpdateAsync(Application application)
        {
            Application record = await GetByIdAsync(application.Id);

            using (QueryFactory db = _queryFactory.Invoke())
            {
                await db.Query("applications").Where("Id", application.Id).UpdateAsync(new
                {
                    Name = application.Name ?? record.Name,
                    RedirectUri = application.RedirectUri ?? record.RedirectUri,
                    Secret = application.Secret ?? record.Secret,
                    Description = application.Description ?? record.Description
                });
            }
        }



        public async Task DeleteAsync(string id)
        {
            using (QueryFactory db = _queryFactory.Invoke())
            {
                await db.Query("applications").Where("Id", id).DeleteAsync();
            }
        }



        public async Task AddDownloadAsync(string id, string downloadId)
        {
            using (QueryFactory db = _queryFactory.Invoke())
            {
                await db.Query("applications").Where("Id", id).UpdateAsync(new
                {
                    DownloadId = downloadId
                });
            }
        }

        public async Task RemoveDownloadAsync(string id)
        {
            using (QueryFactory db = _queryFactory.Invoke())
            {
                await db.Query("applications").Where("Id", id).UpdateAsync(new
                {
                    DownloadId = null as string
                });
            }
        }


        public async Task<string> AddIconAsync(string id, string filename, string contentType, Stream fileStream, CancellationToken cancellationToken = default)
        {
            CloudFile cf = await _storage.UploadFileAsync(fileStream, _options.AppIconStorageBucket, id, filename, contentType, cancellationToken);

            using (QueryFactory db = _queryFactory.Invoke())
            {
                await db.Query("applications").Where("Id", id).UpdateAsync(new
                {
                    IconUrl = cf.MediaLink
                });               
            }

            return cf.MediaLink;
        }

        public async Task RemoveIconAsync(string id)
        {
            await _storage.DeleteFileAsync(_options.AppIconStorageBucket, id);

            using (QueryFactory db = _queryFactory.Invoke())
            {
                await db.Query("applications").Where("Id", id).UpdateAsync(new
                {
                    IconUrl = null as string
                });
            }
        }

        

        public async Task<bool> ValidateAsync(string id, string secret, string redirectUri, bool ignoreSecret)
        {
            var application = await GetByIdAsync(id);

            if (application == null)
                return false;

            bool valid = true;

            valid = (secret != null || ignoreSecret) && valid;
            valid = (application.Secret == secret || ignoreSecret) && valid;

            valid = new Uri(application.RedirectUri) == new Uri(redirectUri) && valid;

            return valid;
        }
    }
}
