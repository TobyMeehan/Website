using DataAccessLibrary.DataAccess;
using DataAccessLibrary.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Storage
{
    public class UserRoleTable : IUserRoleTable
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public UserRoleTable(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        /// <summary>
        /// Selects all user roles of a user
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public async Task<List<UserRoleModel>> SelectByUser(string userid)
        {
            string sql = "SELECT * FROM `userroles` WHERE UserId = @userid";

            object parameters = new
            {
                userid
            };

            return await _sqlDataAccess.LoadData<UserRoleModel>(sql, parameters);
        }

        /// <summary>
        /// Selects all user roles of a role
        /// </summary>
        /// <param name="roleid"></param>
        /// <returns></returns>
        public async Task<List<UserRoleModel>> SelectByRole(string roleid)
        {
            string sql = "SELECT * FROM `userroles` WHERE RoleId = @roleid";

            object parameters = new
            {
                roleid
            };

            return await _sqlDataAccess.LoadData<UserRoleModel>(sql, parameters);
        }

        /// <summary>
        /// Inserts a new user role
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task Insert(UserRoleModel value)
        {
            string sql = "INSERT INTO `userroles` (UserId, RoleId) VALUES (@UserId, @RoleId)";

            await _sqlDataAccess.SaveData(sql, value);
        }

        /// <summary>
        /// Deletes all user roles with the role
        /// </summary>
        /// <param name="roleid"></param>
        /// <returns></returns>
        public async Task DeleteByRole(string roleid)
        {
            string sql = "DELETE FROM `userroles` WHERE RoleId = @roleid";

            object parameters = new
            {
                roleid
            };

            await _sqlDataAccess.SaveData(sql, parameters);
        }

        /// <summary>
        /// Deletes a user roles with the user
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public async Task DeleteByUser(string userid)
        {
            string sql = "DELETE FROM `userroles` WHERE UserId = @userid";

            object parameters = new
            {
                userid
            };

            await _sqlDataAccess.SaveData(sql, parameters);
        }
    }
}
