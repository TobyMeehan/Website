using AutoMapper;
using BlazorUI.Extensions;
using DataAccessLibrary.Data;
using BlazorUI.Models;
using DataAccessLibrary;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace BlazorUI.Pages.Downloads
{
    public partial class Authors : ComponentBase
    {
        [Inject] private IDownloadProcessor downloadProcessor { get; set; }
        [Inject] private IUserProcessor userProcessor { get; set; }
        [Inject] private IMapper mapper { get; set; }
        [Inject] private AuthenticationStateProvider authenticationStateProvider { get; set; }
        [Inject] private IAuthorizationService authorizationService { get; set; }
        [Inject] private NavigationManager navigationManager { get; set; }
        [Inject] private EditDownloadState editDownloadState { get; set; }
        [Inject] private AlertState alertState { get; set; }

        [Parameter] public string Id { get; set; }

        private Download _download;
        private List<User> _users;
        private AuthenticationState _context;

        private Alert _accessDeniedAlert = new Alert
        {
            Title = null,
            Body = "You do not have edit access for this download.",
            Context = AlertContext.Danger
        };

        protected override async Task OnInitializedAsync()
        {
            _context = await authenticationStateProvider.GetAuthenticationStateAsync();
            _download = await Task.Run(async () => mapper.Map<Download>(await downloadProcessor.GetDownloadById(Id)));
            _users = await Task.Run(async () =>
            {
                return mapper.Map<List<User>>(await userProcessor.GetUsersByRole(UserRoles.Verified))
                        .Where(filter => _download.Authors.Any(user => user.Id != filter.Id))
                        .ToList();
            });

            editDownloadState.Title = _download.Title;
            editDownloadState.Id = _download.Id;
        }

        private async Task AddUser_Click(User user)
        {
            if ((await authorizationService.AuthorizeAsync(_context.User, _download, Authorization.Policies.EditDownload)).Succeeded)
            {
                if (_users.Any(filter => filter.Id == user.Id))
                {
                    await downloadProcessor.AddAuthor(_download.Id, user.Id);
                    _download.Authors.Add(user);
                    _users.Remove(user);
                }
            }
            else
            {
                alertState.Queue.Add(_accessDeniedAlert);

                navigationManager.NavigateTo($"/downloads/{Id}");
            }
        }

        private async Task RemoveUser_Click(User user)
        {
            if ((await authorizationService.AuthorizeAsync(_context.User, _download, Authorization.Policies.EditDownload)).Succeeded)
            {
                await downloadProcessor.RemoveAuthor(_download.Id, user.Id);
                _download.Authors.Remove(user);
                _users.Add(user);
            }
            else
            {
                alertState.Queue.Add(_accessDeniedAlert);

                navigationManager.NavigateTo($"/downloads/{Id}");
            }
        }
    }
}
