using DataAccessLibrary.Models;
using System.Threading.Tasks;

namespace DataAccessLibrary.Data
{
    public interface IConnectionProcessor
    {
        Task<AuthorizationCode> CreateAuthorizationCode(string userid, string appid);
        Task DeleteConnection(string connectionid);
        Task<AuthorizationCode> GetAuthorizationCode(string code);
    }
}