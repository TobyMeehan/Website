using DataAccessLibrary.DataAccess;
using DataAccessLibrary.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Storage
{
    public class ObjectiveTable : IObjectiveTable
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public ObjectiveTable(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<List<Objective>> SelectById(string id)
        {
            string sql = "SELECT * FROM `objectives` WHERE `Id` = @id";

            object parameters = new
            {
                id
            };

            return await _sqlDataAccess.LoadData<Objective>(sql, parameters);
        }

        public async Task<List<Objective>> SelectByApplication(string appid)
        {
            string sql = "SELECT * FROM `objectives` WHERE `AppId` = @appid";

            object parameters = new
            {
                appid
            };

            return await _sqlDataAccess.LoadData<Objective>(sql, parameters);
        }

        public async Task Insert(Objective objective)
        {
            string sql = "INSERT INTO `objectives` (Id, AppId, Name) VALUES (UUID(), @AppId, @Name)";

            await _sqlDataAccess.SaveData(sql, objective);
        }

        public async Task Delete(string id)
        {
            string sql = "DELETE FROM `objectives` WHERE `Id` = @id";

            object parameters = new
            {
                id
            };

            await _sqlDataAccess.SaveData(sql, parameters);
        }
    }
}
