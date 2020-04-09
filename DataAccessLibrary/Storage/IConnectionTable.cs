using DataAccessLibrary.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLibrary.Storage
{
    public interface IConnectionTable
    {
        Task Delete(string connectionid);
        Task Insert(Connection connection);
        Task<List<Connection>> SelectById(string connectionid);
        Task<List<Connection>> SelectByUserAndApplication(string userid, string appid);
    }
}