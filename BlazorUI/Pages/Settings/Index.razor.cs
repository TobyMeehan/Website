using BlazorUI.Extensions;
using BlazorUI.Models;
using BlazorUI.Shared;
using DataAccessLibrary.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorUI.Pages.Settings
{
    [Authorize]
    public partial class Index : ComponentBase
    {
        [Inject] private IUserProcessor userProcessor { get; set; }
        [Inject] private AuthenticationStateProvider authenticationStateProvider { get; set; }
        [Inject] private NavigationManager navigationManager { get; set; }

        private UsernameFormModel _usernameForm = new UsernameFormModel();
        private ChangePasswordFormModel _changePasswordForm = new ChangePasswordFormModel();
        private PasswordFormModel _deleteAccountForm = new PasswordFormModel();
        private AuthenticationState _context;
        private bool _passwordSuccess;

        private ServerSideValidator _usernameValidator;
        private ServerSideValidator _changePasswordValidator;
        private ServerSideValidator _deleteValidator;

        protected override async Task OnInitializedAsync()
        {
            _context = await authenticationStateProvider.GetAuthenticationStateAsync();
        }

        private async Task UsernameForm_Submit()
        {
            if (await userProcessor.UserExists(_usernameForm.Username) && _usernameForm.Username != _context.User.GetUsername())
            {
                Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>
                {
                    { nameof(_usernameForm.Username), new List<string> { "That username is already in use." } }
                };
                _usernameValidator.DisplayErrors(errors);
            }
            else
            {
                await userProcessor.UpdateUsername(_context.User.GetUserId(), _usernameForm.Username);

                navigationManager.NavigateTo($"/login?redirectUri={navigationManager.Uri}", true);
            }
        }

        private async Task ChangePasswordForm_Submit()
        {
            Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();
            if (_changePasswordForm.NewPassword == _changePasswordForm.ConfirmNewPassword) // this is checked by the validator but has caused some issues in some versions of blazor
            {
                if (await userProcessor.Authenticate(_context.User.GetUsername(), _changePasswordForm.CurrentPassword))
                {
                    await userProcessor.UpdatePassword(_context.User.GetUserId(), _changePasswordForm.NewPassword);
                }
                else
                {
                    errors.Add(nameof(_changePasswordForm.CurrentPassword), new List<string> { "Incorrect current password." });
                }
            }
            else
            {
                List<string> message = new List<string> { "Passwords do not match." };
                errors.Add(nameof(_changePasswordForm.NewPassword), message);
                errors.Add(nameof(_changePasswordForm.ConfirmNewPassword), message);
            }

            if (errors.Any())
            {
                _changePasswordValidator.DisplayErrors(errors);
            }
            else
            {
                _changePasswordForm = new ChangePasswordFormModel();
                _passwordSuccess = true;
            }
        }

        private async Task DeleteAccountForm_Submit()
        {
            if (await userProcessor.Authenticate(_context.User.GetUsername(), _deleteAccountForm.Password))
            {
                await userProcessor.DeleteUser(_context.User.GetUserId());
                navigationManager.NavigateTo("/logout", true);
            }
            else
            {
                Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>
                {
                    { nameof(_deleteAccountForm.Password), new List<string> { "Incorrect password." } }
                };
                _deleteValidator.DisplayErrors(errors);
            }
        }
    }
}
