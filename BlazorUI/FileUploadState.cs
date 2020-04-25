using BlazorUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorUI
{
    public class FileUploadState
    {
        private void NotifyStateChanged() => OnStateChanged?.Invoke();
        public event Action OnStateChanged;

        public List<FileUpload> Uploads { get; set; } = new List<FileUpload>();
        public void DismissUpload(FileUpload fileUpload)
        {
            Uploads.Remove(fileUpload);
            NotifyStateChanged();
        }

        public async Task UploadFile(FileUpload fileUpload)
        {
            Uploads.Add(fileUpload);
            NotifyStateChanged();

            fileUpload.OnProgressChanged += NotifyStateChanged;

            fileUpload.Status = await fileUpload.Task;

            NotifyStateChanged();
        }
    }
}
