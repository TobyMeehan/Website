using Blazor.FileReader;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TobyMeehan.Com.Components;
using TobyMeehan.Com.Data.Extensions;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories;
using TobyMeehan.Com.Data.Upload;
using TobyMeehan.Com.Extensions;
using TobyMeehan.Com.Models;
using TobyMeehan.Com.Tasks;

namespace TobyMeehan.Com.Pages.Downloads
{
    public partial class Files : ComponentBase
    {
        [Inject] private IDownloadRepository downloads { get; set; }
        [Inject] public IDownloadFileRepository downloadFiles { get; set; }
        [Inject] private ProgressTaskState taskState { get; set; }
        [Inject] private EditDownloadState editDownloadState { get; set; }
        [Inject] private AlertState alertState { get; set; }

        [Parameter] public string Id { get; set; }

        private FilenameFormModel _filenameForm = new FilenameFormModel();
        private ServerSideValidator _serverSideValidator;

        private Download _download;
        private IEnumerable<FileUploadTask> _uploadTasks = new List<FileUploadTask>();
        private readonly int _maxSize = 200 * 1024 * 1024;

        protected override async Task OnInitializedAsync()
        {
            _download = await downloads.GetByIdAsync(Id);

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

                string filename = FormatFilename(fileInfo.Name);

                Stream uploadStream = await file.OpenReadAsync();

                Func<Stream, CancellationToken, IProgress<IUploadProgress>, Task> uploadTask = async (stream, cancellationToken, progress) =>
                {
                    await downloadFiles.AddAsync(_download.Id, filename, stream, cancellationToken, progress);
                };

                var progressTask = new FileUploadTask(filename, _download, uploadStream, uploadTask);

                progressTask.OnComplete += async t => await RefreshFileList();

                taskState.Add(progressTask);
            }
        }

        private string FormatFilename(string filename)
        {
            List<string> extensions = filename.Split(".").ToList();
            string baseName = extensions[0]; 
            extensions.RemoveAt(0);
            string extension = $".{string.Join(".", extensions)}";

            string filenameFormat = "{0}{1}{2}";
            int i = 1;
            while (_download.Files.Any(f => f.Filename == filename))
            {
                filename = string.Format(filenameFormat, baseName, $"({i++})", extension);
            }

            return filename;
        }

        private async Task RefreshFileList()
        {
            var files = await downloadFiles.GetByDownloadAsync(_download.Id);
            _download.Files = files.ToList(); // TODO: not use dtos in ui
            StateHasChanged();
        }

        private async Task DeleteFile_Click(DownloadFile file)
        {
            await downloadFiles.DeleteAsync(file.Id);
            await RefreshFileList();
        }

        private void SetRename(DownloadFile file)
        {
            _filenameForm.Id = file.Id;
            _filenameForm.Filename = file.Filename;
        }

        private async Task RenameFile_Submit()
        {
            await downloadFiles.UpdateFilenameAsync(_filenameForm.Id, FormatFilename(_filenameForm.Filename));
            await RefreshFileList();
        }
    }
}
