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
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Security;
using TobyMeehan.Com.Data.Upload;
using TobyMeehan.Sql;

namespace TobyMeehan.Com.Data.Repositories
{
    public class SqlUserRepository : SqlRepository<User>, IUserRepository
    {
        private readonly ISqlTable<User> _table;
        private readonly ISqlTable<UserRole> _userRoleTable;
        private readonly IPasswordHash _passwordHash;
        private readonly ICloudStorage _cloudStorage;
        private readonly CloudStorageOptions _options;

        public SqlUserRepository(ISqlTable<User> table, ISqlTable<UserRole> userRoleTable, IPasswordHash passwordHash, ICloudStorage cloudStorage, IOptions<CloudStorageOptions> options) : base(table)
        {
            _table = table;
            _userRoleTable = userRoleTable;
            _passwordHash = passwordHash;
            _cloudStorage = cloudStorage;
            _options = options.Value;
        }

        public async Task<User> AddAsync(string username, string password)
        {
            string id = Guid.NewGuid().ToString();

            await _table.InsertAsync(new
            {
                Id = id,
                Username = username,
                HashedPassword = _passwordHash.HashPassword(password)
            });

            return await GetByIdAsync(id);
        }

        public async Task AddProfilePictureAsync(string id, string filename, string contentType, Stream fileStream, CancellationToken cancellationToken = default, IProgress<IUploadProgress> progress = null)
        {
            CloudFile file = await _cloudStorage.UploadFileAsync(fileStream, _options.ProfilePictureStorageBucket, id, filename, contentType, cancellationToken, progress);

            await _table.UpdateAsync(u => u.Id == id, new
            {
                ProfilePictureUrl = file.MediaLink
            });
        }

        public async Task RemoveProfilePictureAsync(string id)
        {
            await _cloudStorage.DeleteFileAsync(_options.ProfilePictureStorageBucket, id);

            string p = null;

            await _table.UpdateAsync(u => u.Id == id, new
            {
                ProfilePictureUrl = p
            });
        }

        public Task AddRoleAsync(string id, string roleId)
        {
            return _userRoleTable.InsertAsync(new UserRole
            {
                UserId = id,
                RoleId = roleId
            });
        }

        public async Task<bool> AnyUsernameAsync(string username)
        {
            return (await GetByUsernameAsync(username)) != null;
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

        public async Task<IList<User>> GetByRoleAsync(string name)
        {
            return (await _table.SelectByAsync<Role>((u, r) => r.Name == name)).ToList();
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            return (await _table.SelectByAsync(x => x.Username == username)).SingleOrDefault();
        }

        public Task RemoveRoleAsync(string id, string roleId)
        {
            return _userRoleTable.DeleteAsync(x => x.UserId == id && x.RoleId == roleId);
        }

        public Task UpdatePasswordAysnc(string id, string password)
        {
            return _table.UpdateAsync(x => x.Id == id, new
            {
                HashedPassword = _passwordHash.HashPassword(password)
            });
        }

        public Task UpdateUsernameAsync(string id, string username)
        {
            return _table.UpdateAsync(x => x.Id == id, new
            {
                Username = username
            });
        }

        public Task UpdateDescriptionAsync(string id, string description)
        {
            return _table.UpdateAsync(u => u.Id == id, new
            {
                Description = description
            });
        }
    }
}
