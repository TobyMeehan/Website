using DataAccessLibrary.Models;
using DataAccessLibrary.Storage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Data
{
    public class ScoreboardProcessor : ProcessorBase, IScoreboardProcessor
    {
        private readonly IObjectiveTable _objectiveTable;
        private readonly IScoreboardTable _scoreboardTable;
        private readonly IUserProcessor _userProcessor;

        public ScoreboardProcessor(IObjectiveTable objectiveTable, IScoreboardTable scoreboardTable, IUserProcessor userProcessor)
        {
            _objectiveTable = objectiveTable;
            _scoreboardTable = scoreboardTable;
            _userProcessor = userProcessor;
        }

        public async Task<Scoreboard> GetScoreboardByApplication(string appid)
        {
            Scoreboard scoreboard = new Scoreboard();

            scoreboard.AppId = appid;
            scoreboard.Objectives = await _objectiveTable.SelectByApplication(appid) ?? new List<Objective>();
            scoreboard.Scores = new List<Score>();

            foreach (Objective objective in scoreboard.Objectives)
            {
                foreach (Score score in await _scoreboardTable.SelectByObjective(objective.Id))
                {
                    score.Objective = objective;
                    score.User = await _userProcessor.GetUserById(score.UserId);
                    scoreboard.Scores.Add(score);
                }
            }

            return scoreboard;
        }

        public async Task<Objective> GetObjectiveById(string id)
        {
            if (ValidateQuery(await _objectiveTable.SelectById(id), out Objective objective))
            {
                return objective;
            }
            else
            {
                return null;
            }
        }

        public async Task CreateObjective(Objective objective)
        {
            await _objectiveTable.Insert(objective);
        }

        public async Task SetScore(string userid, string objectiveid, int value)
        {
            if (ValidateQuery(await _scoreboardTable.SelectByObjectiveAndUser(objectiveid, userid), out Score score))
            {
                score.Value = value;
                await _scoreboardTable.Update(score);
            }
            else
            {
                await _scoreboardTable.Insert(new Score { ObjectiveId = objectiveid, UserId = userid, Value = value });
            }
        }

        public async Task DeleteObjective(string objectiveid)
        {
            await _objectiveTable.Delete(objectiveid);
            await _scoreboardTable.DeleteByObjective(objectiveid);
        }
    }
}
