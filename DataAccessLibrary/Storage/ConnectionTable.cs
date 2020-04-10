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

        public async Task Insert(Connection connection)
        {
            string sql = "INSERT INTO `connections` (AppId, UserId, AuthorizationCode) VALUES (@AppId, @UserId, @AuthorizationCode)";

            await _sqlDataAccess.SaveData(sql, connection);
        }

        public async Task<List<Connection>> SelectById(string connectionid)
        {
            string sql = "SELECT * FROM `connections` WHERE `Id` = @connectionid";

            object parameters = new
            {
                connectionid
            };

            return await _sqlDataAccess.LoadData<Connection>(sql, parameters);
        }

        public async Task<List<Connection>> SelectByUserAndApplication(string userid, string appid)
        {
            string sql = "SELECT * FROM `connections` WHERE `UserId` = @userid AND `AppId` = @appid";

            object parameters = new
            {
                userid,
                appid
            };

            return await _sqlDataAccess.LoadData<Connection>(sql, parameters);
        }

        public async Task<List<Connection>> SelectByUser(string userid)
        {
            string sql = "SELECT * FROM `connections` WHERE `UserId` = @userid";

            object parameters = new
            {
                userid
            };

            return await _sqlDataAccess.LoadData<Connection>(sql, parameters);
        }

        public async Task Delete(string connectionid)
        {
            string sql = "DELETE FROM `connections` WHERE `Id` = @connectionid";

            object parameters = new
            {
                connectionid
            };

            await _sqlDataAccess.SaveData(sql, parameters);
        }
    }
}
