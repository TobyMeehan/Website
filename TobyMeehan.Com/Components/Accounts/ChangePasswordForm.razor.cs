using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TobyMeehan.Com.Data;
using TobyMeehan.Com.Data.Authentication;
using TobyMeehan.Com.Data.Extensions;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Models;

namespace TobyMeehan.Com.Components.Accounts
{
    public partial class ChangePasswordForm : ComponentBase
    {
        [Inject] private IAuthentication<User> authentication { get; set; }
        [Inject] private IRepository<User> users { get; set; }
        [Inject] private NavigationManager navigation { get; set; }

        [Parameter] public User User { get; set; }

        private ChangePasswordViewModel _model;

        private ServerSideValidator _serverSideValidator;

        private async Task Form_ValidSubmit()
        {
            if (!(await authentication.CheckPasswordAsync(User.Username, _model.CurrentPassword)).Success)
            {
                _serverSideValidator.Error(nameof(_model.CurrentPassword), "Incorrect current password.");
            }

            await users.UpdatePasswordAsync(User, _model.NewPassword);
        }
    }
}
