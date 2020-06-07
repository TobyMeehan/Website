using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TobyMeehan.Com.Components.Accounts;
using TobyMeehan.Com.Data;
using TobyMeehan.Com.Data.Extensions;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories;
using TobyMeehan.Com.Extensions;
using TobyMeehan.Com.Models;

namespace TobyMeehan.Com.Pages.Settings
{
    public partial class Index : ComponentBase
    {
        [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        [Inject] private IUserRepository users { get; set; }
        [Inject] private NavigationManager navigation { get; set; }

        [CascadingParameter] public User CurrentUser { get; set; }

        private AccountSettingsForm _form;

        private async Task UsernameForm_Submit(UsernameViewModel model)
        {
            string username = model.Username;

            if (await users.AnyUsernameAsync(username))
            {
                _form.UsernameForm.InvalidUsername("That username is already in use.");

                return;
            }

            await users.UpdateUsernameAsync(CurrentUser.Id, model.Username);

            navigation.NavigateToRefresh();
        }

        private async Task PasswordForm_Submit(ChangePasswordViewModel model)
        {
            if (!(await users.AuthenticateAsync(CurrentUser.Username, model.CurrentPassword)).Success)
            {
                _form.PasswordForm.InvalidCurrentPassword("Incorrect current password.");

                return;
            }

            await users.UpdatePasswordAysnc(CurrentUser.Id, model.NewPassword);

            model = new ChangePasswordViewModel();
        }

        private async Task AccountForm_Submit(PasswordViewModel model)
        {
            if (!(await users.AuthenticateAsync(CurrentUser.Username, model.Password)).Success)
            {
                _form.AccountForm.InvalidPassword("Incorrect password.");

                return;
            }

            await users.DeleteAsync(CurrentUser.Id);

            navigation.NavigateTo("/logout", true);
        }
    }
}
