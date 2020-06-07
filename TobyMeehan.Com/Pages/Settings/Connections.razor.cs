using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TobyMeehan.Com.Data;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories;

namespace TobyMeehan.Com.Pages.Settings
{
    public partial class Connections : ComponentBase
    {
        [Inject] private IConnectionRepository connections { get; set; }

        [CascadingParameter] public User CurrentUser { get; set; }

        private IList<Connection> _connections;

        protected override async Task OnInitializedAsync()
        {
            _connections = await connections.GetByUserAsync(CurrentUser.Id);
        }

        private async Task RevokeConnection(Connection connection)
        {
            await connections.DeleteAsync(connection.Id);
            _connections.Remove(connection);
        }
    }
}
