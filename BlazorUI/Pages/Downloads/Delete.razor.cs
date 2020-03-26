using AutoMapper;
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

namespace BlazorUI.Pages.Downloads
{    
    public partial class Delete : ComponentBase
    {
        [Inject] private IDownloadProcessor downloadProcessor { get; set; }
        [Inject] private IMapper mapper { get; set; }
        [Inject] private AuthenticationStateProvider authenticationStateProvider { get; set; }
        [Inject] private IAuthorizationService authorizationService { get; set; }
        [Inject] private NavigationManager navigationManager { get; set; }
        [Inject] private EditDownloadState editDownloadState { get; set; }
        [Inject] private AlertState alertState { get; set; }

        [Parameter] public string Id { get; set; }

        private Download _download;
        private AuthenticationState _context;

        ProcessButton _confirmButton;

        private Alert _accessDeniedAlert = new Alert
        {
            Title = "Hey!",
            Body = "You can't delete someone else's download, that's just not on.",
            Context = AlertContext.Danger
        };

        protected override async Task OnInitializedAsync()
        {
            _context = await authenticationStateProvider.GetAuthenticationStateAsync();
            _download = await Task.Run(async () => mapper.Map<Download>(await downloadProcessor.GetDownloadById(Id)));

            editDownloadState.Title = _download?.Title;
            editDownloadState.Id = _download?.Id;
        }

        private async Task ConfirmButton_Click()
        {
            _confirmButton.SetProgress(true);

            if ((await authorizationService.AuthorizeAsync(_context.User, Authorization.Policies.EditDownload)).Succeeded)
            {
                await downloadProcessor.DeleteDownload(Id);

                alertState.Queue.Add(new Alert
                {
                    Title = null,
                    Body = $"{_download.Title} has been successfully deleted.",
                    Context = AlertContext.Success
                });

                navigationManager.NavigateTo("/downloads");
            }
            else
            {
                alertState.Queue.Add(_accessDeniedAlert);

                navigationManager.NavigateTo($"/downloads/{Id}");
            }
        }
    }
}
