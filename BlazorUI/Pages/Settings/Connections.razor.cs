﻿using AutoMapper;
using BlazorUI.Extensions;
using BlazorUI.Models;
using DataAccessLibrary.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorUI.Pages.Settings
{
    public partial class Connections : ComponentBase
    {
        [Inject] private IApplicationProcessor applicationProcessor { get; set; }
        [Inject] private IConnectionProcessor connectionProcessor { get; set; }
        [Inject] private IMapper mapper { get; set; }

        private List<Connection> _connections;
        private List<Application> _applications;
        private AuthenticationState _context;

        [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _context = await AuthenticationStateTask;
            _connections = await Task.Run(async () => mapper.Map<List<Connection>>(await connectionProcessor.GetConnectionsByUser(_context.User.GetUserId())));
            _applications = await Task.Run(async () => mapper.Map<List<Application>>(await applicationProcessor.GetApplicationsByUser(_context.User.GetUserId())));
        }

        private async Task RevokeConnection_Click(Connection connection)
        {
            await connectionProcessor.DeleteConnection(connection.Id);
            _connections.Remove(connection);
        }
    }
}
