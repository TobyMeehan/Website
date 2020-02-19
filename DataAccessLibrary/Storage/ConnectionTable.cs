using DataAccessLibrary.DataAccess;
using DataAccessLibrary.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Storage
{
    public class ConnectionTable : IConnectionTable
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public ConnectionTable(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task Insert(ConnectionModel connection)
        {
            string sql = "INSERT INTO `connections` (AppId, UserId, AuthorizationCode) VALUES (@AppId, @UserId, @AuthorizationCode)";

            await _sqlDataAccess.SaveData(sql, connection);
        }

        public async Task<List<ConnectionModel>> SelectByAuthCode(string authorizationCode)
        {
            string sql = "SELECT * FROM `connections` WHERE `AuthorizationCode` = @authorizationCode";

            object parameters = new
            {
                authorizationCode
            };

            return await _sqlDataAccess.LoadData<ConnectionModel>(sql, parameters);
        }

        public async Task<List<ConnectionModel>> SelectByUserAndApplication(string userid, string appid)
        {
            string sql = "SELECT * FROM `connections` WHERE `UserId` = @userid AND `AppId` = @appid";

            object parameters = new
            {
                userid,
                appid
            };

            return await _sqlDataAccess.LoadData<ConnectionModel>(sql, parameters);
        }

        public async Task Update(ConnectionModel connection)
        {
            string sql = "UPDATE `connections` SET `AuthorizationCode` = @AuthorizationCode WHERE `UserId` = @UserId AND `AppId` = @AppId";

            await _sqlDataAccess.SaveData(sql, connection);
        }

        public async Task Delete(string userid, string appid)
        {
            string sql = "UPDATE `connections` SET `AuthorizationCode` = NULL WHERE `UserId` = @userid AND `AppId` = @appid";

            object parameters = new
            {
                userid,
                appid
            };

            await _sqlDataAccess.SaveData(sql, parameters);
        }
    }
}
