using DataAccessLibrary.DataAccess;
using DataAccessLibrary.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Storage
{
    public class ScoreboardTable : IScoreboardTable
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public ScoreboardTable(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<List<Score>> SelectByObjective(string objectiveId)
        {
            string sql = "SELECT * FROM `scoreboard` WHERE `ObjectiveId` = @objectiveId";

            object parameters = new
            {
                objectiveId
            };

            return await _sqlDataAccess.LoadData<Score>(sql, parameters);
        }

        public async Task<List<Score>> SelectByUser(string userId)
        {
            string sql = "SELECT * FROM `scoreboard` WHERE `UserId` = @userId";

            object parameters = new
            {
                userId
            };

            return await _sqlDataAccess.LoadData<Score>(sql, parameters);
        }

        public async Task<List<Score>> SelectByObjectiveAndUser(string objectiveid, string userid)
        {
            string sql = "SELECT * FROM `scoreboard` WHERE `ObjectiveId` = @objectiveid AND `UserId` = @userid";

            object parameters = new
            {
                objectiveid,
                userid
            };

            return await _sqlDataAccess.LoadData<Score>(sql, parameters);
        }

        public async Task Insert(Score score)
        {
            string sql = "INSERT INTO `scoreboard` (Id, ObjectiveId, UserId, Value) VALUES (UUID(), @ObjectiveId, @UserId, @Value)";

            await _sqlDataAccess.SaveData(sql, score);
        }

        public async Task Update(Score score)
        {
            string sql = "UPDATE `scoreboard` SET Value = @Value WHERE Id = @Id";

            await _sqlDataAccess.SaveData(sql, score);
        }

        public async Task Delete(string id)
        {
            string sql = "DELETE FROM `scoreboard` WHERE `Id` = @id";

            object parameters = new
            {
                id
            };

            await _sqlDataAccess.SaveData(sql, parameters);
        }

        public async Task DeleteByObjective(string objectiveid)
        {
            string sql = "DELETE FROM `scoreboard` WHERE `ObjectiveId` = @objectiveid";

            object parameters = new
            {
                objectiveid
            };

            await _sqlDataAccess.SaveData(sql, parameters);
        }

        public async Task DeleteByUser(string userid)
        {
            string sql = "DELETE FROM `scoreboard` WHERE `UserId` = @userid";

            object parameters = new
            {
                userid
            };

            await _sqlDataAccess.SaveData(sql, parameters);
        }
    }
}
