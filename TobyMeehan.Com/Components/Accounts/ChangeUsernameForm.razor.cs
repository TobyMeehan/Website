using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TobyMeehan.Com.Data;
using TobyMeehan.Com.Data.Extensions;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Extensions;
using TobyMeehan.Com.Models;

namespace TobyMeehan.Com.Components.Accounts
{
    public partial class ChangeUsernameForm : ComponentBase
    {
        [Inject] private IRepository<User> users { get; set; }
        [Inject] private NavigationManager navigation { get; set; }

        [CascadingParameter] public Task<User> UserTask { get; set; }

        private User _user;
        private UsernameViewModel _model = new UsernameViewModel();

        private ServerSideValidator _serverSideValidator;

        protected override async Task OnInitializedAsync()
        {
            _user = await UserTask;
            _model.Username = _user.Username;
        }

        private async Task Form_ValidSubmit()
        {
            if (await users.AnyAsync(u => u.Username == _model.Username))
            {
                _serverSideValidator.Error(nameof(_model.Username), "That username is already in use.");

                return;
            }

            await users.UpdateUsernameAsync(_user, _model.Username);

            navigation.NavigateToRefresh();
        }
    }
}
