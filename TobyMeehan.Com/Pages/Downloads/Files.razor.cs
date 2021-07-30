using Blazor.FileReader;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Extensions;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories;
using TobyMeehan.Com.Data.Upload;
using TobyMeehan.Com.Extensions;
using TobyMeehan.Com.Tasks;

namespace TobyMeehan.Com.Pages.Downloads
{
    public partial class Files : ComponentBase
    {
        [Inject] private IDownloadRepository downloads { get; set; }
        [Inject] public IDownloadFileRepository downloadFiles { get; set; }
        [Inject] private ProgressTaskState taskState { get; set; }
        [Inject] private EditDownloadState editDownloadState { get; set; }

        [Parameter] public string Id { get; set; }

        private Download _download;
        private List<string> _files = new List<string>();
        private IEnumerable<FileUploadTask> _uploadTasks = new List<FileUploadTask>();
        private readonly int _maxSize = 200 * 1024 * 1024;

        protected override async Task OnInitializedAsync()
        {
            _download = await downloads.GetByIdAsync(Id);
            _files = _download.Files.Select(f => f.Filename).ToList();

            editDownloadState.Id = _download.Id;
            editDownloadState.Title = _download.Title;

            taskState.OnStateChanged += async () =>
            {
                _uploadTasks = taskState.Tasks
                    .Where(t => t is FileUploadTask file && file.Download == _download && file.Status != Tasks.TaskStatus.Completed)
                    .Select(f => f as FileUploadTask);

                await RefreshFileList();
            };
        }

        private async Task FileUpload_Change(IEnumerable<IFileReference> files)
        {
            foreach (var file in files)
            {
                var fileInfo = await file.ReadFileInfoAsync();

                if (fileInfo.Size > _maxSize)
                {
                    return;
                }

                Stream uploadStream = await file.OpenReadAsync();

                Func<Stream, CancellationToken, IProgress<IUploadProgress>, Task> uploadTask = async (stream, cancellationToken, progress) =>
                {
                    await downloadFiles.AddAsync(_download.Id, fileInfo.Name, stream, cancellationToken, progress);
                };

                var progressTask = new FileUploadTask(fileInfo.Name, _download, uploadStream, uploadTask);

                progressTask.OnComplete += async t => await RefreshFileList();

                taskState.Add(progressTask);
            }
        }

        private async Task RefreshFileList()
        {
            _download.Files = await downloadFiles.GetByDownloadAsync(_download.Id);
            StateHasChanged();
        }

        private async Task DeleteFile_Click(DownloadFile file)
        {
            await downloadFiles.DeleteAsync(file.Id);
            await RefreshFileList();
        }
    }
}
