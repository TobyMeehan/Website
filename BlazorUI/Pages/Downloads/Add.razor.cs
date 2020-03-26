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

namespace BlazorUI.Pages.Downloads
{
    [Authorize(Policy = Authorization.Policies.IsVerified)]
    public partial class Add : ComponentBase
    {
        [Inject] private IDownloadProcessor downloadProcessor { get; set; }
        [Inject] private IMapper mapper { get; set; }
        [Inject] private AuthenticationStateProvider authenticationStateProvider { get; set; }
        [Inject] private IAuthorizationService authorizationService { get; set; }
        [Inject] private NavigationManager navigationManager { get; set; }
        [Inject] private AlertState alertState { get; set; }

        private DownloadFormModel _downloadForm = new DownloadFormModel();
        private AuthenticationState _context;

        ProcessButton _submitButton;

        protected override async Task OnInitializedAsync()
        {
            _context = await authenticationStateProvider.GetAuthenticationStateAsync();
        }

        private async Task DownloadForm_Submit()
        {
            _submitButton.SetProgress(true);

            if ((await authorizationService.AuthorizeAsync(_context.User, Authorization.Policies.IsVerified)).Succeeded)
            {
                var download = new Download
                {
                    Title = _downloadForm.Title,
                    ShortDescription = _downloadForm.ShortDescription,
                    LongDescription = _downloadForm.LongDescription,
                    Verified = DataAccessLibrary.Models.DownloadVerification.None,
                    Version = "", // TODO: version
                    CreatorId = _context.User.GetUserId()
                };

                string id = (await downloadProcessor.CreateDownload(mapper.Map<DataAccessLibrary.Models.Download>(download))).Id;

                navigationManager.NavigateTo($"/downloads/{id}/files");
            }
            else
            {
                alertState.Queue.Add(new Alert
                {
                    Title = null,
                    Body = "Sorry, you need to be verified to create downloads.",
                    Context = AlertContext.Danger
                });

                navigationManager.NavigateTo($"/downloads");
            }
        }
    }
}
