using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TobyMeehan.Com.Data;
using TobyMeehan.Com.Data.Models;

namespace TobyMeehan.Com.Pages.Settings
{
    public partial class Connections : ComponentBase
    {
        [Inject] private IRepository<Connection> connections { get; set; }

        [CascadingParameter] public User CurrentUser { get; set; }

        private List<Connection> _connections;

        protected override async Task OnInitializedAsync()
        {
            _connections = (await connections.GetByAsync<User>((c, u) => c.UserId == $"{CurrentUser.Id}")).ToList();
        }

        private async Task RevokeConnection(Connection connection)
        {
            await connections.RemoveByIdAsync(connection.Id);
            _connections.Remove(connection);
        }
    }
}
