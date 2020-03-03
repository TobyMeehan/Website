using DataAccessLibrary.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLibrary.Storage
{
    public interface IConnectionTable
    {
        Task Insert(Connection connection);
        Task<List<Connection>> SelectByAuthCode(string authorizationCode);
        Task<List<Connection>> SelectByUserAndApplication(string userid, string appid);
        Task Update(Connection connection);
        Task Delete(string userid, string appid);
    }
}