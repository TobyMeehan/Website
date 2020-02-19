using DataAccessLibrary.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLibrary.Storage
{
    public interface IConnectionTable
    {
        Task Insert(ConnectionModel connection);
        Task<List<ConnectionModel>> SelectByAuthCode(string authorizationCode);
        Task<List<ConnectionModel>> SelectByUserAndApplication(string userid, string appid);
        Task Update(ConnectionModel connection);
        Task Delete(string userid, string appid);
    }
}