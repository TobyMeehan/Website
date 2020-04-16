using DataAccessLibrary.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLibrary.Storage
{
    public interface IObjectiveTable
    {
        Task Delete(string id);
        Task Insert(Objective objective);
        Task<List<Objective>> SelectByApplication(string appid);
        Task<List<Objective>> SelectById(string id);
    }
}