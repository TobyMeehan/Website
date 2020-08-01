using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Sql;

namespace TobyMeehan.Com.Data.Repositories
{
    public class SqlApplicationRepository : SqlRepository<Application>, IApplicationRepository
    {
        private readonly ISqlTable<Application> _table;

        public SqlApplicationRepository(ISqlTable<Application> table) : base(table)
        {
            _table = table;
        }

        public async Task<Application> AddAsync(string userId, string name, string redirectUri, string secret = null)
        {
            if ((await GetByUserAndNameAsync(userId, name)) != null)
            {
                throw new ArgumentException("Application with the same name has already been created by the user.", nameof(name));
            }

            string id = Guid.NewGuid().ToString();

            await _table.InsertAsync(new
            {
                UserId = userId,
                Name = name,
                RedirectUri = redirectUri,
                Secret = secret
            });

            return (await _table.SelectByAsync(a => a.Id == id)).SingleOrDefault();
        }

        public async Task<IList<Application>> GetByNameAsync(string name)
        {
            return (await _table.SelectByAsync(a => a.Name == name)).ToList();
        }

        public async Task<Application> GetByUserAndNameAsync(string userId, string name)
        {
            return (await _table.SelectByAsync(a => a.UserId == userId && a.Name == name)).SingleOrDefault();
        }

        public async Task<IList<Application>> GetByUserAsync(string userId)
        {
            return (await _table.SelectByAsync(a => a.UserId == userId)).ToList();
        }

        public Task UpdateAsync(Application application)
        {
            return _table.UpdateAsync(a => a.Id == $"{application.Id}", new
            {
                application.Name,
                application.RedirectUri,
                application.Secret,
                application.Role
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

            valid = application.RedirectUri == redirectUri && valid;

            return valid;
        }
    }
}
