using SqlKata;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Collections;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories;
using TobyMeehan.Com.Data.Security;

namespace TobyMeehan.Com.Data.SqlKata
{
    public class DownloadRepository : RepositoryBase<Download>, IDownloadRepository
    {
        private readonly Func<QueryFactory> _queryFactory;
        private readonly IUserRepository _users;

        public DownloadRepository(Func<QueryFactory> queryFactory, IUserRepository users) : base(queryFactory)
        {
            _queryFactory = queryFactory;
            _users = users;
        }

        protected override Query Query()
        {
            var files = new Query("downloadfiles").OrderBy("Filename");

            return base.Query()
                .From("downloads")
                .OrderByDesc("Updated")
                .OrderBy("Title")
                .LeftJoin(files.As("files"), j => j.On("files.DownloadId", "downloads.Id"));
        }

        protected override async Task<IEntityCollection<Download>> MapAsync(IEnumerable<Download> items)
        {
            foreach (var item in items)
            {
                item.Authors = new EntityCollection<User>(await _users.GetByDownloadAsync(item.Id));
            }

            return await base.MapAsync(items);
        }



        public async Task<Download> AddAsync(string title, string shortDescription, string longDescription, Version version, string userId)
        {
            using (QueryFactory db = _queryFactory.Invoke())
            {
                string id = await db.Query("downloads").InsertGetIdAsync<string>(new
                {
                    Id = RandomString.GeneratePseudo(),
                    Title = title,
                    ShortDescription = shortDescription,
                    LongDescription = longDescription,
                    VersionString = version.ToString(),
                    Updated = DateTime.Now
                });

                return await GetByIdAsync(id);
            }
        }



        public async Task<IEntityCollection<Download>> GetAsync()
        {
            return await SelectAsync();
        }

        public async Task<IEntityCollection<Download>> GetByAuthorAsync(string userId)
        {
            var author = new Query("downloadauthors").Select("DownloadId").Where("UserId", userId);

            return await SelectAsync(query => query.WhereIn("downloads.Id", author));
        }

        public async Task<Download> GetByIdAsync(string id)
        {
            return await SelectSingleAsync(query => query.Where("downloads.Id", id));
        }



        public async Task<Download> UpdateAsync(string id, Download download)
        {
            var record = await GetByIdAsync(id);

            if (download.Version != null && download.Version > record.Version)
            {
                download.Updated = DateTime.Now;
            }
            else
            {
                download.Updated = record.Updated;
                download.Version = record.Version;
            }

            using (QueryFactory db = _queryFactory.Invoke())
            {
                await db.Query("downloads").Where("Id", id).UpdateAsync(new
                {
                    Title = download.Title ?? record.Title,
                    ShortDescription = download.ShortDescription ?? record.ShortDescription,
                    LongDescription = download.LongDescription ?? record.LongDescription,
                    download.VersionString,
                    download.Updated,
                    download.Visibility
                });
            }

            return await GetByIdAsync(id);
        }

        public async Task VerifyAsync(string id, DownloadVerification verification)
        {
            using (QueryFactory db = _queryFactory.Invoke())
            {
                await db.Query("downloads").Where("Id", id).UpdateAsync(new
                {
                    Verified = verification
                });
            }
        }



        public async Task DeleteAsync(string id)
        {
            using (QueryFactory db = _queryFactory.Invoke())
            {
                await db.Query("downloads").Where("Id", id).DeleteAsync();
            }
        }



        public async Task AddAuthorAsync(string id, string userId)
        {
            using (QueryFactory db = _queryFactory.Invoke())
            {
                await db.Query("downloadauthors").InsertAsync(new
                {
                    DownloadId = id,
                    UserId = userId
                });
            }
        }

        public async Task RemoveAuthorAsync(string id, string userId)
        {
            using (QueryFactory db = _queryFactory.Invoke())
            {
                await db.Query("downloadauthors").Where("DownloadId", id).Where("UserId", userId).DeleteAsync();
            }
        }
    }
}
