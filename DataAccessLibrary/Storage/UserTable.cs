using DataAccessLibrary.DataAccess;
using DataAccessLibrary.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Storage
{
    public class UserTable : IUserTable
    {

        private readonly ISqlDataAccess _sqlDataAccess;

        public UserTable(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        /// <summary>
        /// Selects all users
        /// </summary>
        /// <returns></returns>
        public async Task<List<UserModel>> Select()
        {
            string sql = "SELECT * FROM `users`";

            return await _sqlDataAccess.LoadData<UserModel>(sql);
        }

        /// <summary>
        /// Selects a user from their id
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public async Task<List<UserModel>> SelectById(string userid)
        {
            string sql = "SELECT * FROM `users` WHERE `UserId` = @userid";

            object parameters = new
            {
                userid
            };

            return await _sqlDataAccess.LoadData<UserModel>(sql, parameters);
        }

        /// <summary>
        /// Selects a user from their username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<List<UserModel>> SelectByUsername(string username)
        {
            string sql = "SELECT * FROM `users` WHERE `Username` = @username";

            object parameters = new
            {
                username
            };

            return await _sqlDataAccess.LoadData<UserModel>(sql, parameters);
        }

        public async Task<List<PasswordModel>> SelectPassword(string username)
        {
            string sql = "SELECT Username, HashedPassword FROM `users` WHERE `Username` = @username";

            object parameters = new
            {
                username
            };

            return await _sqlDataAccess.LoadData<PasswordModel>(sql, parameters);
        }

        /// <summary>
        /// Inserts a new user. Provided password should be hashed
        /// </summary>
        /// <param name="user"></param>
        /// <param name="hashedPassword"></param>
        /// <returns></returns>
        public async Task Insert(UserModel user, string hashedPassword)
        {
            string sql = "INSERT INTO `users` (UserId, Username, Email, HashedPassword) VALUES (UUID(), @Username, @Email, @hashedPassword)";

            object parameters = new
            {
                user.Username,
                user.Email,
                hashedPassword
            };

            await _sqlDataAccess.SaveData(sql, parameters);
        }

        /// <summary>
        /// Updates the user's username to the new one provided
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task UpdateUsername(string userid, string username)
        {
            string sql = "UPDATE `users` SET Username = @username WHERE UserId = @userid";

            object parameters = new
            {
                userid,
                username
            };

            await _sqlDataAccess.SaveData(sql, parameters);
        }

        /// <summary>
        /// Updates the user's email address to the new one provided
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task UpdateEmail(string userid, string email)
        {
            string sql = "UPDATE `users` SET Email = @email WHERE UserId = @userid";

            object parameters = new
            {
                userid,
                email
            };

            await _sqlDataAccess.SaveData(sql, parameters);
        }

        /// <summary>
        /// Updates the user's password to the new one provided. Provided password should be hashed
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="hashedPassword"></param>
        /// <returns></returns>
        public async Task UpdatePassword(string userid, string hashedPassword)
        {
            string sql = "UPDATE `users` SET HashedPassword = @hashedPassword WHERE UserId = @userid";

            object parameters = new
            {
                userid,
                hashedPassword
            };

            await _sqlDataAccess.SaveData(sql, parameters);
        }

        /// <summary>
        /// Removes the user
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public async Task Delete(string userid)
        {
            string sql = "DELETE FROM `users` WHERE UserId = @userid";

            object parameters = new
            {
                userid
            };

            await _sqlDataAccess.SaveData(sql, parameters);
        }
    }
}
