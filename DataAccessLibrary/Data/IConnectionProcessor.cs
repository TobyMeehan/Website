using DataAccessLibrary.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLibrary.Data
{
    public interface IConnectionProcessor
    {
        Task<AuthorizationCode> CreateAuthorizationCode(string userid, string appid, string codeChallenge = null);
        Task DeleteConnection(string connectionid);
        Task DeleteInvalidAuthCodes();
        Task<AuthorizationCode> GetAuthorizationCode(string code);
        Task<Connection> GetConnectionByUserAndApplication(string userid, string appid);
        Task<List<Connection>> GetConnectionsByUser(string userid);
        bool CheckPkce(string codeChallenge, string codeVerifier);
    }
}