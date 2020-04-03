using AutoMapper;
using BlazorUI.Extensions;
using BlazorUI.Models;
using BlazorUI.Shared;
using DataAccessLibrary.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorUI.Pages.Users
{
    public partial class Details : ComponentBase
    {
        [Inject] private IUserProcessor userProcessor { get; set; }
        [Inject] private IDownloadProcessor downloadProcessor { get; set; }
        [Inject] private IRoleProcessor roleProcessor { get; set; }
        [Inject] private IMapper mapper { get; set; }
        [Inject] private AuthenticationStateProvider authenticationStateProvider { get; set; }
        [Inject] private IAuthorizationService authorizationService { get; set; }
        [Inject] private AlertState alertState { get; set; }

        [Parameter] public string Username { get; set; }

        private User _user;
        private List<Role> _roles;
        private List<Download> _downloads;
        private ChangePasswordFormModel _adminPasswordForm = new ChangePasswordFormModel();
        private AuthenticationState _context;

        private ServerSideValidator _adminPasswordValidator;

        protected override async Task OnInitializedAsync()
        {
            _context = await authenticationStateProvider.GetAuthenticationStateAsync();
            _user = await Task.Run(async () => mapper.Map<User>(await userProcessor.GetUserByUsername(Username)));
            _roles = await Task.Run(async () =>
            {
                return mapper.Map<List<Role>>(await roleProcessor.GetRoles())
                        .Where(filter => !_user.Roles.Any(role => role.Id == filter.Id))
                        .ToList();
            });
            _downloads = await Task.Run(async () => mapper.Map<List<Download>>(await downloadProcessor.GetDownloadsByAuthor(_user.Id)));
        }

        async Task AdminPasswordForm_Submit()
        {
            Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();
            if (_adminPasswordForm.NewPassword == _adminPasswordForm.ConfirmNewPassword) // see /Settings/Index.razor #189
            {
                if (await userProcessor.Authenticate(_context.User.GetUsername(), _adminPasswordForm.CurrentPassword))
                {
                    await userProcessor.UpdatePassword(_user.Id, _adminPasswordForm.NewPassword);

                    alertState.Queue.Add(new Alert
                    {
                        Title = null,
                        Body = $"Successfully changed password of {_user.Username}.",
                        Context = AlertContext.Success
                    });

                    _adminPasswordForm = new ChangePasswordFormModel();
                }
                else
                {
                    errors.Add(nameof(_adminPasswordForm.CurrentPassword), new List<string> { "Admin password was incorrect. " });
                }
            }
            else
            {
                List<string> message = new List<string> { "Passwords do not match." };
                errors.Add(nameof(_adminPasswordForm.NewPassword), message);
                errors.Add(nameof(_adminPasswordForm.ConfirmNewPassword), message);
            }

            if (errors.Any())
            {
                _adminPasswordValidator.DisplayErrors(errors);
            }
        }

        async Task AddRole_Click(Role role)
        {
            if ((await authorizationService.AuthorizeAsync(_context.User, Authorization.Policies.IsAdmin)).Succeeded)
            {
                await userProcessor.AddRole(_user.Id, mapper.Map<DataAccessLibrary.Models.Role>(role));
                _user.Roles.Add(role);
                _roles.Remove(role);
            }
        }

        async Task RemoveRole_Click(Role role)
        {
            if ((await authorizationService.AuthorizeAsync(_context.User, Authorization.Policies.IsAdmin)).Succeeded)
            {
                await userProcessor.RemoveRole(_user.Id, mapper.Map<DataAccessLibrary.Models.Role>(role));
                _user.Roles.Remove(role);
                _roles.Add(role);
            }
        }
    }
}
