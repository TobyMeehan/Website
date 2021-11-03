using SqlKata;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Collections;
using TobyMeehan.Com.Data.Extensions;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories;

namespace TobyMeehan.Com.Data.SqlKata
{
    public class LobbyRepository : RepositoryBase<Lobby>, ILobbyRepository
    {
        private readonly Func<QueryFactory> _queryFactory;
        private readonly IUserRepository _users;

        public LobbyRepository(Func<QueryFactory> queryFactory, IUserRepository users): base(queryFactory)
        {
            _queryFactory = queryFactory;
            _users = users;
        }

        protected override Query Query()
        {
            var connections = new Query("lobbyconnections").OrderBy("DisplayName");

            return base.Query()
                .From("lobbies")
                .OrderBy("Name")
                .LeftJoin(connections.As("conns"), j => j.On("conns.LobbyId", "lobbies.Id"))

                .Select("lobbies.{Id, Name}",
                        "conns.Id AS Connections_Id", "conns.UserId AS Connections_UserId", "conns.DisplayName AS Connections_DisplayName");
        }

        protected async override Task<IEntityCollection<Lobby>> MapAsync(IEnumerable<Lobby> items)
        {
            foreach (var lobby in items)
            {
                foreach (var connection in lobby.Connections)
                {
                    if (connection.UserId != null)
                    {
                        connection.User = await _users.GetByIdAsync(connection.UserId);
                    }
                }
            }

            return await base.MapAsync(items);
        }

        public async Task<Lobby> AddAsync(string name)
        {
            string id = Guid.NewGuid().ToToken();

            using (QueryFactory db = _queryFactory.Invoke())
            {
                await db.Query("lobbies").InsertAsync(new
                {
                    Id = id,
                    Name = name
                });
            }

            return await GetByIdAsync(id);
        }

        public async Task<LobbyConnection> AddConnectionAsync(string connectionId, string lobbyId, string displayName)
        {
            using (QueryFactory db = _queryFactory.Invoke())
            {
                await db.Query("lobbyconnections").InsertAsync(new
                {
                    Id = connectionId,
                    LobbyId = lobbyId,
                    DisplayName = displayName
                });
            }

            return null;
        }

        public async Task DeleteAsync(string id)
        {
            using (QueryFactory db = _queryFactory.Invoke())
            {
                await db.Query("lobbies").Where("Id", id).DeleteAsync();
            }
        }

        public async Task<Lobby> GetByIdAsync(string id)
        {
            return await SelectSingleAsync(query => query.Where("lobbies.Id", id));
        }

        public async Task RemoveConnectionAsync(string connectionId)
        {
            using (QueryFactory db = _queryFactory.Invoke())
            {
                await db.Query("lobbyconnections").Where("Id", connectionId).DeleteAsync();
            }
        }
    }
}
