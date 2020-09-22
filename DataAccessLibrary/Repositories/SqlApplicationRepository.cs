using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.CloudStorage;
using TobyMeehan.Com.Data.Configuration;
using TobyMeehan.Com.Data.Extensions;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Sql;

namespace TobyMeehan.Com.Data.Repositories
{
    public class SqlApplicationRepository : SqlRepository<Application>, IApplicationRepository
    {
        private readonly ISqlTable<Application> _table;
        private readonly ICloudStorage _cloudStorage;
        private readonly IDownloadRepository _downloads;
        private readonly CloudStorageOptions _options;

        public SqlApplicationRepository(ISqlTable<Application> table, ICloudStorage cloudStorage, IDownloadRepository downloads, IOptions<CloudStorageOptions> options) : base(table)
        {
            _table = table;
            _cloudStorage = cloudStorage;
            _downloads = downloads;
            _options = options.Value;
        }

        protected override async Task<IEnumerable<Application>> FormatAsync(IEnumerable<Application> values)
        {
            foreach (var application in values)
            {
                application.Download = await _downloads.GetByIdAsync(application.DownloadId);
            }

            return await base.FormatAsync(values);
        }

        public async Task<Application> AddAsync(string userId, string name, string redirectUri, bool secret)
        {
            string id = Guid.NewGuid().ToString();

            await _table.InsertAsync(new
            {
                Id = id,
                UserId = userId,
                Name = name,
                RedirectUri = redirectUri,
                Secret = secret ? Guid.NewGuid().ToToken() : null
            });

            return (await _table.SelectByAsync(a => a.Id == id)).SingleOrDefault();
        }

        public async Task AddDownloadAsync(string id, string downloadId)
        {
            await _table.UpdateAsync(a => a.Id == id, new
            {
                DownloadId = downloadId
            });
        }

        public async Task<string> AddIconAsync(string id, string filename, string contentType, Stream fileStream, CancellationToken cancellationToken = default)
        {
            CloudFile file = await _cloudStorage.UploadFileAsync(fileStream, _options.AppIconStorageBucket, id, filename, contentType, cancellationToken);

            await _table.UpdateAsync(a => a.Id == id, new
            {
                IconUrl = file.MediaLink
            });

            return file.MediaLink;
        }

        public async Task<IList<Application>> GetByNameAsync(string name)
        {
            return (await SelectAsync(a => a.Name == name)).ToList();
        }

        public async Task<Application> GetByUserAndNameAsync(string userId, string name)
        {
            return (await SelectAsync(a => a.UserId == userId && a.Name == name)).SingleOrDefault();
        }

        public async Task<IList<Application>> GetByUserAsync(string userId)
        {
            return (await SelectAsync(a => a.UserId == userId)).ToList();
        }

        public Task RemoveDownloadAsync(string id)
        {
            return _table.UpdateAsync(a => a.Id == id, new
            {
                DownloadId = null as string
            });
        }

        public async Task RemoveIconAsync(string id)
        {
            await _cloudStorage.DeleteFileAsync(_options.AppIconStorageBucket, id);

            await _table.UpdateAsync(a => a.Id == id, new
            {
                IconUrl = null as string
            });
        }

        public async Task UpdateAsync(Application application)
        {
            Application record = await GetByIdAsync(application.Id);

            await _table.UpdateAsync(a => a.Id == $"{application.Id}", new
            {
                Name = application.Name ?? record.Name,
                RedirectUri = application.RedirectUri ?? record.RedirectUri,
                Secret = application.Secret ?? record.Secret,
                Description = application.Description ?? record.Description
            });
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
