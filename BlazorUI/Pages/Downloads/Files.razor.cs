using AutoMapper;
using BlazorInputFile;
using BlazorUI.Models;
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
    public partial class Files : ComponentBase
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

        private Alert _accessDeniedAlert = new Alert
        {
            Title = null,
            Body = "You do not have edit access for this download.",
            Context = AlertContext.Danger
        };

        protected override async Task OnInitializedAsync()
        {
            _context = await authenticationStateProvider.GetAuthenticationStateAsync();
            _download = await Task.Run(async () => mapper.Map<Models.Download>(await downloadProcessor.GetDownloadById(Id)));

            editDownloadState.Title = _download.Title;
            editDownloadState.Id = _download.Id;
        }

        private async Task FileUpload_Change(IFileListEntry[] files)
        {
            if ((await authorizationService.AuthorizeAsync(_context.User, _download, Authorization.Policies.EditDownload)).Succeeded)
            {
                const int maxSize = 209715200; // 200MB
                var file = files.FirstOrDefault();
                byte[] contents = (await file.ReadAllAsync(maxSize)).ToArray();
                await downloadProcessor.CreateFile(new DataAccessLibrary.Models.DownloadFileModel { DownloadId = _download.Id, Filename = file.Name }, contents);

                _download.Files.Add(file.Name);
            }
            else
            {
                alertState.Queue.Add(_accessDeniedAlert);

                navigationManager.NavigateTo($"/downloads/{Id}");
            }
        }

        private async Task DeleteFile_Click(string filename)
        {
            if ((await authorizationService.AuthorizeAsync(_context.User, _download, Authorization.Policies.EditDownload)).Succeeded)
            {
                await downloadProcessor.DeleteFile(new DataAccessLibrary.Models.DownloadFileModel { DownloadId = _download.Id, Filename = filename });
                _download.Files.Remove(filename);
            }
            else
            {
                alertState.Queue.Add(_accessDeniedAlert);

                navigationManager.NavigateTo($"/downloads/{Id}");
            }
        }
    }
}
