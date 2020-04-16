using DataAccessLibrary.Models;
using System.Threading.Tasks;

namespace DataAccessLibrary.Data
{
    public interface IScoreboardProcessor
    {
        Task CreateObjective(Objective objective);
        Task DeleteObjective(string objectiveid);
        Task<Scoreboard> GetScoreboardByApplication(string appid);
        Task<Objective> GetObjectiveById(string id);
        Task SetScore(string userid, string objectiveid, int value);
    }
}