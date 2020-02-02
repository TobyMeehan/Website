using DataAccessLibrary.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLibrary.Data
{
    public interface IUserProcessor
    {
        Task<bool> Authenticate(string username, string password);
        Task<UserModel> CreateUser(UserModel user, string password, List<RoleModel> roles);
        Task<UserModel> GetUserById(string userid);
        Task<UserModel> GetUserByUsername(string username);
        Task<bool> UserExists(string username);
    }
}