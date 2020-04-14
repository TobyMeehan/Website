using AutoMapper;
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
    public partial class Transactions : ComponentBase
    {
        [Inject] private IUserProcessor userProcessor { get; set; }
        [Inject] private IMapper mapper { get; set; }

        [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        private User _user;
        private AuthenticationState _context;

        protected override async Task OnInitializedAsync()
        {
            _context = await AuthenticationStateTask;
            _user = await Task.Run(async () => mapper.Map<User>(await userProcessor.GetUserById(_context.User.GetUserId())));
        }
    }
}
