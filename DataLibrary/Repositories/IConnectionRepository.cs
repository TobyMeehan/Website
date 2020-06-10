using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Models;

namespace TobyMeehan.Com.Data.Repositories
{
    public interface IConnectionRepository
    {
        Task<IList<Connection>> GetAsync();

        Task<Connection> GetByIdAsync(string id);

        Task<IList<Connection>> GetByUserAsync(string userId);

        Task<IList<Connection>> GetByApplicationAsync(string appId);

        Task<Connection> GetByUserAndApplicationAsync(string userId, string appId);

        Task<AuthorizationCode> GetByAuthorizationCodeAsync(string code);

        Task<AuthorizationCode> AddAsync(string userId, string appId, string codeChallenge = null);

        Task DeleteAsync(string id);
    }
}
