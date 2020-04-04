using AutoMapper;
using BlazorUI.Models;
using DataAccessLibrary.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorUI.Pages.Downloads
{
    public partial class Details : ComponentBase
    {
        [Inject] public IConfiguration configuration { get; set; }
        [Inject] private IDownloadProcessor downloadProcessor { get; set; }
        [Inject] private IMapper mapper { get; set; }
        [Inject] private AuthenticationStateProvider authenticationStateProvider { get; set; }
        [Inject] private IAuthorizationService authorizationService { get; set; }
        [Inject] private AlertState alertState { get; set; }

        [Parameter] public string Id { get; set; }

        private Download _download;
        private string _downloadHost;
        private AuthenticationState _context;

        protected override async Task OnInitializedAsync()
        {
            _context = await authenticationStateProvider.GetAuthenticationStateAsync();
            _download = await Task.Run(async () => mapper.Map<Download>(await downloadProcessor.GetDownloadById(Id)));
            _downloadHost = configuration.GetSection("DownloadHost").Value;
        }

        private async Task VerifyForm_Submit()
        {
            if ((await authorizationService.AuthorizeAsync(_context.User, Authorization.Policies.IsAdmin)).Succeeded)
            {
                await downloadProcessor.VerifyDownload(_download.Id, _download.Verified);

                alertState.Queue.Add(new Alert
                {
                    Title = null,
                    Body = $"Successfully changed verification to {_download.Verified}.",
                    Context = AlertContext.Success
                });
            }
        }
    }
}
