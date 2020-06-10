using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Models;

namespace TobyMeehan.Com.Data.Repositories
{
    public interface IUserRepository
    {
        Task<IList<User>> GetAsync();

        Task<IList<User>> GetByRoleAsync(string name);

        Task<User> GetByIdAsync(string id);

        Task<User> GetByUsernameAsync(string username);

        Task<AuthenticationResult<User>> AuthenticateAsync(string username, string password);

        Task<User> AddAsync(string username, string password);

        Task AddRoleAsync(string id, string roleId);

        Task RemoveRoleAsync(string id, string roleId);

        Task<bool> AnyUsernameAsync(string username);

        Task UpdateUsernameAsync(string id, string username);

        Task UpdatePasswordAysnc(string id, string password);

        Task DeleteAsync(string id);
    }
}
