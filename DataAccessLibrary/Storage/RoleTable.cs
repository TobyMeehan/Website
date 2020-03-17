using DataAccessLibrary.DataAccess;
using DataAccessLibrary.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Storage
{
    public class RoleTable : IRoleTable
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public RoleTable(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        /// <summary>
        /// Selects all roles
        /// </summary>
        /// <returns></returns>
        public async Task<List<Role>> Select()
        {
            string sql = "SELECT * FROM `roles`";

            return await _sqlDataAccess.LoadData<Role>(sql);
        }

        /// <summary>
        /// Selects a role from its id
        /// </summary>
        /// <param name="roleid"></param>
        /// <returns></returns>
        public async Task<List<Role>> SelectById(string roleid)
        {
            string sql = "SELECT * FROM `roles` WHERE `Id` = @roleid";

            object parameters = new
            {
                roleid
            };

            return await _sqlDataAccess.LoadData<Role>(sql, parameters);
        }

        /// <summary>
        /// Selects a role from its name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<List<Role>> SelectByName(string name)
        {
            string sql = "SELECT * FROM `roles` WHERE `Name` = @name";

            object parameters = new
            {
                name
            };

            return await _sqlDataAccess.LoadData<Role>(sql, parameters);
        }

        /// <summary>
        /// Inserts a new role
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task Insert(string name)
        {
            string sql = "INSERT INTO `roles` (Id, Name) VALUES (UUID(), @name)";

            object parameters = new
            {
                name
            };

            await _sqlDataAccess.SaveData(sql, parameters);
        }

        /// <summary>
        /// Removes the role
        /// </summary>
        /// <param name="roleid"></param>
        /// <returns></returns>
        public async Task Delete(string roleid)
        {
            string sql = "DELETE FROM `roles` WHERE `Id` = @roleid";

            object parameters = new
            {
                roleid
            };

            await _sqlDataAccess.SaveData(sql, parameters);
        }
    }
}
