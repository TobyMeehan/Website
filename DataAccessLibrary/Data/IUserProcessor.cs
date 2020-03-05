using DataAccessLibrary.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLibrary.Data
{
    public interface IUserProcessor
    {
        Task<bool> Authenticate(string username, string password);
        Task<User> CreateUser(User user, string password);
        Task<User> GetUserById(string userid);
        Task<User> GetUserByUsername(string username);
        Task<bool> UserExists(string username);
        Task DeleteUser(string userid);
    }
}