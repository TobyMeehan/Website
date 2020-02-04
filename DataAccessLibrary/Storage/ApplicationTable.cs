using DataAccessLibrary.DataAccess;
using DataAccessLibrary.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Storage
{
    public class ApplicationTable : IApplicationTable
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public ApplicationTable(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<List<ApplicationModel>> SelectById(string appid)
        {
            string sql = "SELECT * FROM `applications` WHERE `AppId` = @appid";

            object parameters = new
            {
                appid
            };

            return await _sqlDataAccess.LoadData<ApplicationModel>(sql, parameters);
        }

        public async Task<List<ApplicationModel>> SelectByName(string name)
        {
            string sql = "SELECT * FROM `applications` WHERE `Name` = @name";

            object parameters = new
            {
                name
            };

            return await _sqlDataAccess.LoadData<ApplicationModel>(sql, parameters);
        }

        public async Task<List<ApplicationModel>> SelectByUser(string userid)
        {
            string sql = "SELECT * FROM `applications` WHERE `UserId` = @userid";

            object parameters = new
            {
                userid
            };

            return await _sqlDataAccess.LoadData<ApplicationModel>(sql, parameters);
        }

        public async Task<List<ApplicationModel>> SelectByUserAndName(string userid, string name)
        {
            string sql = "SELECT * FROM `applications` WHERE `UserId` = @userid AND `Name` = @name";

            object parameters = new
            {
                userid,
                name
            };

            return await _sqlDataAccess.LoadData<ApplicationModel>(sql, parameters);
        }

        public async Task Insert(ApplicationModel app)
        {
            string sql = "INSERT INTO `applications` (AppId, UserId, Name, RedirectUri, Secret) VALUES (UUID(), @UserId, @Name, @RedirectUri, @Secret)";

            await _sqlDataAccess.SaveData(sql, app);
        }

        public async Task Delete(string appid)
        {
            string sql = "DELETE FROM `applications` WHERE `AppId` = @appid";

            object parameters = new
            {
                appid
            };

            await _sqlDataAccess.SaveData(sql, parameters);
        }
    }
}
