using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TobyMeehan.Com.Components;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories;
using TobyMeehan.Com.Extensions;
using TobyMeehan.Com.Models;

namespace TobyMeehan.Com.Pages.Users
{
    public partial class Details : ComponentBase
    {
        [Inject] private IUserRepository users { get; set; }
        [Inject] private IRoleRepository roles { get; set; }
        [Inject] private IDownloadRepository downloads { get; set; }
        [Inject] private AlertState alertState { get; set; }

        [Parameter] public string Url { get; set; }

        [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        private User _user;
        private IList<Role> _unusedRoles;
        private IList<Download> _downloads;
        private ChangePasswordViewModel _passwordForm = new ChangePasswordViewModel();
        private AuthenticationState _context;

        private ServerSideValidator _serverSideValidator;

        protected override async Task OnInitializedAsync()
        {
            await RefreshRoles();

            var list = await Task.Run(() => downloads.GetByAuthorAsync(_user.Id));
            _downloads = list.Where(d => d.Visibility == DownloadVisibility.Public).ToList();

            _context = await AuthenticationStateTask;
        }

        private async Task AdminPasswordForm_Submit()
        {
            if (!(await users.AuthenticateAsync(_context.User.Username(), _passwordForm.CurrentPassword)).Success)
            {
                _serverSideValidator.Error(nameof(_passwordForm.CurrentPassword), "Admin password was incorrect.");
                return;
            }

            await users.UpdatePasswordAysnc(_user.Id, _passwordForm.NewPassword);

            alertState.Add(new AlertModel { Context = BootstrapContext.Success, ChildContent = alertContent });

            _passwordForm = new ChangePasswordViewModel();
        }

        private async Task AddRole(Role role)
        {
            await users.AddRoleAsync(_user.Id, role.Id);
            await RefreshRoles();
        }

        private async Task RemoveRole(Role role)
        {
            await users.RemoveRoleAsync(_user.Id, role.Id);
            await RefreshRoles();
        }

        private async Task RefreshRoles()
        {
            _user = await Task.Run(() => users.GetByVanityUrlAsync(Url));
            _unusedRoles = (await Task.Run(roles.GetAsync))
                .Where(x => !_user.Roles.Any(r => r == x))
                .ToList();
        }
    }
}
