using DataAccessLibrary.Models;
using System.Threading.Tasks;

namespace DataAccessLibrary.Data
{
    public interface IConnectionProcessor
    {
        Task<Connection> CreateConnection(Connection connection);
        Task<Connection> GetConnectionByAuthCode(string authorizationCode);
        Task<bool> ValidateAuthCode(string authorizationCode);
    }
}