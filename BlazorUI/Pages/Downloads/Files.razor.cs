using AutoMapper;
using Blazor.FileReader;
using BlazorUI.Models;
using DataAccessLibrary.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorUI.Pages.Downloads
{
    public partial class Files : ComponentBase
    {
        [Inject] private IConfiguration configuration { get; set; }
        [Inject] private IDownloadProcessor downloadProcessor { get; set; }
        [Inject] private IMapper mapper { get; set; }
        [Inject] private AuthenticationStateProvider authenticationStateProvider { get; set; }
        [Inject] private IAuthorizationService authorizationService { get; set; }
        [Inject] private NavigationManager navigationManager { get; set; }
        [Inject] private EditDownloadState editDownloadState { get; set; }
        [Inject] private FileUploadState uploadState { get; set; }
        [Inject] private AlertState alertState { get; set; }

        [Parameter] public string Id { get; set; }

        private Download _download;
        private string _downloadHost;
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
            _downloadHost = configuration.GetSection("DownloadHost").Value;

            editDownloadState.Title = _download.Title;
            editDownloadState.Id = _download.Id;

            uploadState.OnStateChanged += async () =>
            {
                List<string> files = uploadState.Uploads
                    .Where(x =>
                        x.Status == UploadFileResult.Success &&
                        x.Download == _download.Id &&
                        !_download.Files.Contains(x.Filename))
                    .Select(x => x.Filename)
                    .ToList();

                _download.Files.AddRange(files);

                await InvokeAsync(StateHasChanged);
            };
        }

        private async Task FileUpload_Change(IFileReference[] files)
        {
            if ((await authorizationService.AuthorizeAsync(_context.User, _download, Authorization.Policies.EditDownload)).Succeeded)
            {
                var file = files.FirstOrDefault();
                var fileInfo = await file.ReadFileInfoAsync();

                Progress<int> progress = new Progress<int>();
                CancellationTokenSource cts = new CancellationTokenSource();

                await Task.Run(async () =>
                {
                    var upload = new FileUpload(fileInfo.Name, _download.Id, UploadFile(file, fileInfo, progress, cts.Token), progress, cts);
                    await uploadState.UploadFile(upload);
                });
            }
            else
            {
                alertState.Queue.Add(_accessDeniedAlert);

                navigationManager.NavigateTo($"/downloads/{Id}");
            }
        }

        private async Task<UploadFileResult> UploadFile(IFileReference file, IFileInfo info, IProgress<int> progress, CancellationToken cancellationToken)
        {
            const int maxSize = 100 * 1024 * 1024; // 100MB
            const int bufferSize = 512 * 1024; // 500KB

            if (info.Size > maxSize)
                return UploadFileResult.Failed; // TODO: more interactive logic

            UploadFileResult result;

            await using (var stream = await file.OpenReadAsync())
            {
                result = await downloadProcessor.TryAddFile(new DataAccessLibrary.Models.DownloadFileModel
                {
                    DownloadId = _download.Id,
                    Filename = info.Name
                }, stream, bufferSize, progress, cancellationToken);
            }

            GC.Collect(); // TODO: remove asap

            return result;
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
