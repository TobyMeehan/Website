using BlazorUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorUI
{
    public class FileUploadState
    {
        private void NotifyUploadStart() => OnUploadStart?.Invoke();
        public event Action OnUploadStart;
        private void NotifyUploadComplete(string filename, string download) => OnUploadComplete?.Invoke(filename, download);
        public event Action<string, string> OnUploadComplete;
        private void NotifyUploadFailed() => OnUploadFailed?.Invoke();
        public event Action OnUploadFailed;
        private void NotifyUploadProgress() => OnUploadProgress?.Invoke();
        public event Action OnUploadProgress;

        public List<string> FailedUploads { get; set; } = new List<string>();
        public void DismissFailedUpload(string filename)
        {
            FailedUploads.Remove(filename);
        }

        public List<FileUpload> Uploads { get; set; } = new List<FileUpload>();

        public async Task UploadFile(FileUpload fileUpload)
        {
            Uploads.Add(fileUpload);
            NotifyUploadStart();

            fileUpload.OnProgressChanged += NotifyUploadProgress;

            bool success = await fileUpload.Task;

            if (success)
            {
                Uploads.Remove(fileUpload);
                NotifyUploadComplete(fileUpload.Filename, fileUpload.Download);
            }
            else
            {
                Uploads.Remove(fileUpload);
                FailedUploads.Add(fileUpload.Filename);
                NotifyUploadFailed();
            }
        }
    }
}
