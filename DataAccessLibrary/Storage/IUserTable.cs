using DataAccessLibrary.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLibrary.Storage
{
    public interface IUserTable
    {
        Task Delete(string userid);
        Task Insert(UserModel user, string hashedPassword);
        Task<List<UserModel>> Select();
        Task<List<UserModel>> SelectById(string userid);
        Task<List<UserModel>> SelectByUsername(string username);
        Task<List<PasswordModel>> SelectPassword(string username);
        Task UpdateEmail(string userid, string email);
        Task UpdatePassword(string userid, string hashedPassword);
        Task UpdateUsername(string userid, string username);
    }
}