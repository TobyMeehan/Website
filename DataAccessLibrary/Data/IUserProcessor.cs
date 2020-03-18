using DataAccessLibrary.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLibrary.Data
{
    public interface IUserProcessor
    {
        Task<bool> Authenticate(string username, string password);
        Task AddRole(string userid, Role role);
        Task<User> CreateUser(User user, string password);
        Task UpdateUsername(string userid, string username);
        Task UpdatePassword(string userid, string newPassword);
        Task<User> GetUserById(string userid);
        Task<User> GetUserByUsername(string username);
        Task<List<User>> GetUsersByRole(string rolename);
        Task<bool> UserExists(string username);
        Task DeleteUser(string userid);
    }
}