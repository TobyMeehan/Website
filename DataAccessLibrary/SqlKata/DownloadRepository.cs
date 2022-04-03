using SqlKata;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Security;

namespace TobyMeehan.Com.Data.SqlKata
{
    public class DownloadRepository : RepositoryBase<IDownload, Download, NewDownload, EditDownload>, IDownloadRepository
    {
        private readonly IUserRepository _users;
        private readonly IDownloadFileRepository _files;

        public DownloadRepository(QueryFactory queryFactory, IIdGenerator idGenerator, IUserRepository users, IDownloadFileRepository files) : base(queryFactory, idGenerator, "downloads")
        {
            _users = users;
            _files = files;
        }

        protected override Query Query()
        {
            return base.Query()
                .From(Table)
                .OrderByDesc("Updated")
                .OrderBy("Title");
        }

        protected override async Task<IReadOnlyList<IDownload>> MapAsync(IEnumerable<Download> items)
        {
            var downloads = items.ToList();
            
            foreach (var download in downloads)
            {
                download.Files = await _files.GetByDownloadAsync(download.Id);
                download.Authors = await _users.GetByDownloadAsync(download.Id);
            }

            return await base.MapAsync(downloads);
        }

        public async Task<IReadOnlyList<IDownload>> GetByAuthorAsync(Id<IUser> userId)
        {
            var author = new Query("downloadauthors").Select("DownloadId").Where("UserId", userId);

            return await SelectAsync(query => query.WhereIn($"{Table}.Id", author));
        }

        public async Task AddAuthorAsync(Id<IDownload> id, Id<IUser> userId)
        {
            await QueryFactory.Query("downloadauthors").InsertAsync(new {DownloadId = id.Value, UserId = userId.Value});
        }

        public async Task RemoveAuthorAsync(Id<IDownload> id, Id<IUser> userId)
        {
            await QueryFactory.Query("downloadauthors").Where("DownloadId", id.Value).Where("UserId", userId.Value)
                .DeleteAsync();
        }
    }
}
