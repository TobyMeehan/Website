using Microsoft.Extensions.Options;
using SqlKata;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.CloudStorage;
using TobyMeehan.Com.Data.Collections;
using TobyMeehan.Com.Data.Configuration;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories;
using TobyMeehan.Com.Data.Security;
using TobyMeehan.Com.Data.Upload;

namespace TobyMeehan.Com.Data.SqlKata
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        private readonly Func<QueryFactory> _queryFactory;
        private readonly IPasswordHash _passwordHash;
        private readonly ICloudStorage _cloudStorage;
        private readonly ITransactionRepository _transactions;
        private readonly CloudStorageOptions _options;

        public UserRepository(Func<QueryFactory> queryFactory, IPasswordHash passwordHash, ICloudStorage cloudStorage, IOptions<CloudStorageOptions> options, ITransactionRepository transactions) : base(queryFactory)
        {
            _queryFactory = queryFactory;
            _passwordHash = passwordHash;
            _cloudStorage = cloudStorage;
            _transactions = transactions;
            _options = options.Value;
        }

        protected override Query Query()
        {
            var userroles = new Query("userroles");
            var roles = new Query("roles");

            return base.Query()
                .From("users")
                .OrderBy("Username")
                .LeftJoin(userroles.As("userroles"), j => j.On("userroles.UserId", "users.Id"))
                .LeftJoin(roles.As("roles"), j => j.On("roles.Id", "userroles.RoleId"))

                .Select("users.{Id, Username, VanityUrl, Balance, Description, ProfilePictureUrl}",
                        "roles.Id AS Roles_Id", "roles.Name AS Roles_Name");
        }



        public async Task<User> AddAsync(string username, string password)
        {
            using (QueryFactory db = _queryFactory.Invoke())
            {
                string id = await db.Query("users").InsertGetIdAsync<string>(new
                {
                    Id = Guid.NewGuid().ToString(),
                    Username = username,
                    HashedPassword = _passwordHash.HashPassword(password)
                });

                return await GetByIdAsync(id);
            }
        }



        public async Task<IEntityCollection<User>> GetAsync()
        {
            return await SelectAsync();
        }

        public async Task<User> GetByIdAsync(string id)
        {
            return await SelectSingleAsync(query => query.Where("users.Id", id));
        }

        public async Task<IEntityCollection<User>> GetByRoleAsync(string name)
        {
            var roles = new Query("userroles").Select("UserId").Where("Name", name);

            return await SelectAsync(query => query.WhereIn("users.Id", roles));
        }

        public async Task<IEntityCollection<User>> GetByDownloadAsync(string downloadId)
        {
            var authors = new Query("downloadauthors").Select("UserId").Where("DownloadId", downloadId);

            return await SelectAsync(query => query.WhereIn("users.Id", authors));
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            return await SelectSingleAsync(query => query.Where("Username", username));
        }

        public async Task<User> GetByVanityUrlAsync(string url)
        {
            return await SelectSingleAsync(query => query.Where("VanityUrl", url));
        }



        public async Task UpdateDescriptionAsync(string id, string description)
        {
            using (QueryFactory db = _queryFactory.Invoke())
            {
                await db.Query("users").Where("Id", id).UpdateAsync(new
                {
                    Description = description
                });
            }
        }

        public async Task UpdatePasswordAysnc(string id, string password)
        {
            using (QueryFactory db = _queryFactory.Invoke())
            {
                await db.Query("users").Where("Id", id).UpdateAsync(new
                {
                    HashedPassword = _passwordHash.HashPassword(password)
                });
            }
        }

        public async Task UpdateUsernameAsync(string id, string username)
        {
            if (await AnyUsernameAsync(username))
            {
                throw new ArgumentException("Username already exists.", nameof(username));
            }

            using (QueryFactory db = _queryFactory.Invoke())
            {
                await db.Query("users").Where("Id", id).UpdateAsync(new
                {
                    Username = username
                });
            }
        }

        public async Task UpdateVanityUrlAsync(string id, string vanityUrl)
        {
            if (!new Regex(@"^[a-zA-Z0-9]+$").IsMatch(vanityUrl))
            {
                throw new ArgumentException("Invalid URL string.", nameof(vanityUrl));
            }

            using (QueryFactory db = _queryFactory.Invoke())
            {
                await db.Query("users").Where("Id", id).UpdateAsync(new
                {
                    VanityUrl = vanityUrl
                });
            }
        }



        public async Task DeleteAsync(string id)
        {
            using (QueryFactory db = _queryFactory.Invoke())
            {
                await db.Query("users").Where("Id", id).DeleteAsync();
            }
        }




        public async Task AddProfilePictureAsync(string id, string filename, string contentType, Stream fileStream, CancellationToken cancellationToken = default, IProgress<IUploadProgress> progress = null)
        {
            CloudFile file = await _cloudStorage.UploadFileAsync(fileStream, _options.ProfilePictureStorageBucket, id, filename, contentType, cancellationToken, progress);

            using (QueryFactory db = _queryFactory.Invoke())
            {
                await db.Query("users").Where("Id", id).UpdateAsync(new
                {
                    ProfilePictureUrl = file.MediaLink
                });
            }
        }

        public async Task RemoveProfilePictureAsync(string id)
        {
            await _cloudStorage.DeleteFileAsync(_options.ProfilePictureStorageBucket, id);

            using (QueryFactory db = _queryFactory.Invoke())
            {
                await db.Query("users").Where("users.Id", id).UpdateAsync(new
                {
                    ProfilePictureUrl = (string)null
                });
            }
        }


        public async Task AddRoleAsync(string id, string roleId)
        {
            using (QueryFactory db = _queryFactory.Invoke())
            {
                await db.Query("userroles").InsertAsync(new
                {
                    UserId = id,
                    RoleId = roleId
                });
            }
        }

        public async Task RemoveRoleAsync(string id, string roleId)
        {
            using (QueryFactory db = _queryFactory.Invoke())
            {
                await db.Query("userroles").Where("UserId", id).Where("RoleId", roleId).DeleteAsync();
            }
        }


        public async Task<Transaction> AddTransactionAsync(string id, string appId, string description, int amount)
        {
            var transaction = await _transactions.AddAsync(id, appId, description, amount);

            using (QueryFactory db = _queryFactory.Invoke())
            {
                // https://github.com/sqlkata/querybuilder/issues/159
                await db.StatementAsync("UPDATE users SET Balance = Balance + @Amount WHERE Id = @Id", new
                {
                    Amount = amount,
                    Id = id
                });
            }

            return transaction;
        }



        public async Task<bool> AnyUsernameAsync(string username)
        {
            using (QueryFactory db = _queryFactory.Invoke())
            {
                return await db.Query("users").Where("Username", username).ExistsAsync();
            }
        }

        public async Task<bool> AnyVanityUrlAsync(string vanityUrl)
        {
            using (QueryFactory db = _queryFactory.Invoke())
            {
                return await db.Query("users").Where("VanityUrl", vanityUrl).ExistsAsync();
            }
        }



        public async Task<AuthenticationResult<User>> AuthenticateAsync(string username, string password)
        {
            User user;

            using (QueryFactory db = _queryFactory.Invoke())
            {
                user = await db.Query("users").Where("Username", username).Select("HashedPassword").FirstOrDefaultAsync<User>();
            }

            if (user == null)
            {
                return new AuthenticationResult<User>();
            }

            if (!_passwordHash.CheckPassword(password, user?.HashedPassword))
            {
                return new AuthenticationResult<User>();
            }

            return new AuthenticationResult<User>(await GetByUsernameAsync(username));
        }
    }
}
