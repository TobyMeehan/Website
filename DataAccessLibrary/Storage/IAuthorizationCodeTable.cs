using DataAccessLibrary.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLibrary.Storage
{
    public interface IAuthorizationCodeTable
    {
        Task DeleteByConnection(string connectionid);
        Task DeleteById(string id);
        Task Insert(AuthorizationCode value);
        Task<List<AuthorizationCode>> Select();
        Task<List<AuthorizationCode>> SelectByCode(string code);
    }
}