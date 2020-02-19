using DataAccessLibrary.Models;
using System.Threading.Tasks;

namespace DataAccessLibrary.Data
{
    public interface IConnectionProcessor
    {
        Task<ConnectionModel> CreateConnection(ConnectionModel connection);
        Task<ConnectionModel> GetConnectionByAuthCode(string authorizationCode);
        Task<bool> ValidateAuthCode(string authorizationCode);
    }
}