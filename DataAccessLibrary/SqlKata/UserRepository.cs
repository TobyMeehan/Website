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
using TobyMeehan.Com.Data.Configuration;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories;
using TobyMeehan.Com.Data.Security;
using TobyMeehan.Com.Data.Upload;

namespace TobyMeehan.Com.Data.SqlKata
{
    public class UserRepository : IUserRepository
    {
        private readonly Func<QueryFactory> _queryFactory;
        private readonly IPasswordHash _passwordHash;
        private readonly ICloudStorage _cloudStorage;
        private readonly ITransactionRepository _transactions;
        private readonly CloudStorageOptions _options;

        public UserRepository(Func<QueryFactory> queryFactory, IPasswordHash passwordHash, ICloudStorage cloudStorage, IOptions<CloudStorageOptions> options, ITransactionRepository transactions)
        {
            _queryFactory = queryFactory;
            _passwordHash = passwordHash;
            _cloudStorage = cloudStorage;
            _transactions = transactions;
            _options = options.Value;
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

        public async Task AddRoleAsync(string id, string roleId)
        {
            using (QueryFactory db = _queryFactory.Invoke())
            {
                await db.Query("users").InsertAsync(new
                {
                    UserId = id,
                    RoleId = roleId
                });
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
            return (await GetByUsernameAsync(username)) != null;
        }

        public async Task<bool> AnyVanityUrlAsync(string vanityUrl)
        {
            return (await GetByVanityUrlAsync(vanityUrl)) != null;
        }

        public async Task<AuthenticationResult<User>> AuthenticateAsync(string username, string password)
        {
            User user = await GetByUsernameAsync(username);

            if (user == null)
            {
                return new AuthenticationResult<User>();
            }

            if (!_passwordHash.CheckPassword(password, user?.HashedPassword))
            {
                return new AuthenticationResult<User>();
            }

            return new AuthenticationResult<User>(user);
        }

        public async Task DeleteAsync(string id)
        {
            using (QueryFactory db = _queryFactory.Invoke())
            {
                await db.Query("users").Where("Id", id).DeleteAsync();
            }
        }

        private Query UserQuery() => new Query("users")
            .Select("users.Id, " +
            "users.Username, " +
            "users.VanityUrl, " +
            "users.Balance, " +
            "users.Description" +
            "users.ProfilePictureUrl" +
            "roles.Id" +
            "roles.Name")
            .LeftJoin("userroles", "users.Id", "userroles.UserId")
            .LeftJoin("roles", "userroles.RoleId", "roles.Id");

        public async Task<IList<User>> GetAsync()
        {
            using (QueryFactory db = _queryFactory.Invoke())
            {
                return (await db.GetAsync<User>(UserQuery())).ToList();
            }
        }

        public async Task<User> GetByIdAsync(string id)
        {
            using (QueryFactory db = _queryFactory.Invoke())
            {
                return await db.FirstOrDefaultAsync<User>(UserQuery().Where("users.Id", id));
            }
        }

        public async Task<IList<User>> GetByRoleAsync(string name)
        {
            using (QueryFactory db = _queryFactory.Invoke())
            {
                return (await db.GetAsync<User>(UserQuery().Where("roles.Name", name))).ToList();
            }
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            using (QueryFactory db = _queryFactory.Invoke())
            {
                return await db.FirstOrDefaultAsync<User>(UserQuery().Where("users.Username", username));
            }
        }

        public async Task<User> GetByVanityUrlAsync(string url)
        {
            using (QueryFactory db = _queryFactory.Invoke())
            {
                return await db.FirstOrDefaultAsync<User>(UserQuery().Where("users.VanityUrl", url));
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

        public async Task RemoveRoleAsync(string id, string roleId)
        {
            using (QueryFactory db = _queryFactory.Invoke())
            {
                await db.Query("userroles").Where("UserId", id).Where("RoleId", roleId).DeleteAsync();
            }
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
    }
}
