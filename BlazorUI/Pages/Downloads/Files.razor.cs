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

        private const int _maxSize = 100 * 1024 * 1024; // 100MB
        const int _bufferSize = 512 * 1024; // 500KB

        protected override async Task OnInitializedAsync()
        {
            _context = await authenticationStateProvider.GetAuthenticationStateAsync();
            _download = await Task.Run(async () => mapper.Map<Download>(await downloadProcessor.GetDownloadById(Id)));
            _downloadHost = configuration.GetSection("DownloadHost").Value;

            editDownloadState.Title = _download.Title;
            editDownloadState.Id = _download.Id;

            uploadState.OnStateChanged += async () => await InvokeAsync(StateHasChanged);
            uploadState.OnUploadComplete += async (fileUpload) =>
            {
                if (fileUpload.Download == _download.Id == !_download.Files.Contains(fileUpload.Filename))
                {
                    _download.Files.Add(fileUpload.Filename);
                    await InvokeAsync(StateHasChanged);
                }
            };
        }

        private async Task FileUpload_Change(IFileReference[] files)
        {
            if ((await authorizationService.AuthorizeAsync(_context.User, _download, Authorization.Policies.EditDownload)).Succeeded)
            {
                foreach (var file in files)
                {
                    var fileInfo = await file.ReadFileInfoAsync();

                    if (fileInfo.Size > _maxSize)
                    {
                        return;
                    }

                    Progress<int> progress = new Progress<int>();
                    CancellationTokenSource cts = new CancellationTokenSource();

                    var uploadTask = GetUploadTask(fileInfo.Name, progress, cts.Token);

                    var upload = new FileUpload(fileInfo.Name, _download.Id, uploadTask, await file.OpenReadAsync(), progress, cts);

                    uploadState.Enqueue(upload);
                }
            }
            else
            {
                alertState.Queue.Add(_accessDeniedAlert);

                navigationManager.NavigateTo($"/downloads/{Id}");
            }
        }

        private Func<Stream, Task<UploadFileResult>> GetUploadTask(string filename, IProgress<int> progress, CancellationToken cancellationToken) 
        {
            return (stream) => downloadProcessor.TryAddFile(new DataAccessLibrary.Models.DownloadFileModel
            {
                Filename = filename,
                DownloadId = _download.Id
            }, stream, _bufferSize, progress, cancellationToken);
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
