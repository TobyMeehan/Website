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
        private void NotifyUploadComplete(string filename) => OnUploadComplete?.Invoke(filename);
        public event Action<string> OnUploadComplete;
        private void NotifyUploadFailed(string filename) => OnUploadFailed?.Invoke(filename);
        public event Action<string> OnUploadFailed;

        public List<string> Files { get; set; } = new List<string>();

        public List<string> FailedUploads { get; set; } = new List<string>();
        public void DismissFailedUpload(string filename)
        {
            FailedUploads.Remove(filename);
        }

        public async Task UploadFile(string filename, Task<bool> uploadTask)
        {
            Files.Add(filename);
            NotifyUploadStart();

            bool success = await uploadTask;

            if (success)
            {
                Files.Remove(filename);
                NotifyUploadComplete(filename);
            }
            else
            {
                Files.Remove(filename);
                FailedUploads.Add(filename);
                NotifyUploadFailed(filename);
            }
        }
    }
}
