using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TobyMeehan.Com.Data;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories;

namespace TobyMeehan.Com.Pages.Downloads
{
    public partial class Authors : ComponentBase
    {
        [Inject] private IDownloadRepository downloads { get; set; }
        [Inject] private IUserRepository users { get; set; }
        [Inject] private EditDownloadState editDownloadState { get; set; }

        [Parameter] public string Id { get; set; }

        private Download _download;
        private List<User> _users;

        protected override async Task OnInitializedAsync()
        {
            _download = await Task.Run(() => downloads.GetByIdAsync(Id));
            await RefreshAuthors();

            editDownloadState.Id = _download.Id;
            editDownloadState.Title = _download.Title;
        }

        private async Task RefreshAuthors()
        {
            _download.Authors = (await downloads.GetByIdAsync(Id)).Authors;
            _users = (await users.GetByRoleAsync(UserRoles.Verified))
                .Where(u => !_download.Authors.Any(x => x == u)).ToList();
            StateHasChanged();
        }

        private async Task AddUser(User user)
        {
            await downloads.AddAuthorAsync(_download.Id, user.Id);
            await RefreshAuthors();
        }

        private async Task RemoveUser(User user)
        {
            await downloads.RemoveAuthorAsync(_download.Id, user.Id);
            await RefreshAuthors();
        }
    }
}
