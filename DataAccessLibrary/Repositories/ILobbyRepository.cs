using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Models;

namespace TobyMeehan.Com.Data.Repositories
{
    public interface ILobbyRepository
    {
        Task<Lobby> GetByIdAsync(string id);

        Task<Lobby> AddAsync(string name);

        Task DeleteAsync(string id);

        Task<LobbyConnection> AddConnectionAsync(string connectionId, string lobbyId, string displayName);

        Task RemoveConnectionAsync(string connectionId);
    }
}
