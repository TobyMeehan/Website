using DataAccessLibrary.DataAccess;
using DataAccessLibrary.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Storage
{
    public class AuthorizationCodeTable : IAuthorizationCodeTable
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public AuthorizationCodeTable(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<List<AuthorizationCode>> SelectByCode(string code)
        {
            string sql = "SELECT * FROM `authorizationcodes` WHERE `Code` = @code";

            object parameters = new
            {
                code
            };

            return await _sqlDataAccess.LoadData<AuthorizationCode>(sql, parameters);
        }

        public async Task Insert(AuthorizationCode value)
        {
            string sql = "INSERT INTO `authorizationcodes` (Id, ConnectionId, Code, Expiry) VALUES (UUID(), @ConnectionId, @Code, @Expiry)";

            await _sqlDataAccess.SaveData(sql, value);
        }

        public async Task DeleteById(string id)
        {
            string sql = "DELETE FROM `authorizationcodes` WHERE `Id` = @Id";

            object parameters = new
            {
                id
            };

            await _sqlDataAccess.SaveData(sql, parameters);
        }

        public async Task DeleteByConnection(string connectionid)
        {
            string sql = "DELETE FROM `authorizationcodes` WHERE `ConnectionId` = @connectionid";

            object parameters = new
            {
                connectionid
            };

            await _sqlDataAccess.SaveData(sql, parameters);
        }
    }
}
