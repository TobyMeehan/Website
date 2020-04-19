using DataAccessLibrary.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLibrary.Storage
{
    public interface IScoreboardTable
    {
        Task Delete(string id);
        Task DeleteByObjective(string objectiveid);
        Task DeleteByUser(string userid);
        Task Insert(Score score);
        Task<List<Score>> SelectByObjective(string objectiveId);
        Task<List<Score>> SelectByUser(string userId);
        Task<List<Score>> SelectByObjectiveAndUser(string objectiveid, string userid);
        Task Update(Score score);
    }
}