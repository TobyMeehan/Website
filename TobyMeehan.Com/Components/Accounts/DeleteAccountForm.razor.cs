using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TobyMeehan.Com.Data;
using TobyMeehan.Com.Data.Authentication;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Extensions;
using TobyMeehan.Com.Models;

namespace TobyMeehan.Com.Components.Accounts
{
    public partial class DeleteAccountForm : ComponentBase
    {
        [Inject] private IAuthentication<User> authentication { get; set; }
        [Inject] private IRepository<User> users { get; set; }
        [Inject] private NavigationManager navigation { get; set; }

        [CascadingParameter] public Task<User> UserTask { get; set; }

        private User _user;
        private PasswordViewModel _model = new PasswordViewModel();

        private ServerSideValidator _serverSideValidator;

        protected override async Task OnInitializedAsync()
        {
            _user = await UserTask;
        }

        private async Task Form_ValidSubmit()
        {
            if (!(await authentication.CheckPasswordAsync(_user.Username, _model.Password)).Success)
            {
                _serverSideValidator.Error(nameof(_model.Password), "Incorrect password.");
            }

            await users.RemoveByIdAsync(_user.Id);

            navigation.NavigateToLogout();
        }
    }
}
