using BlazorUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorUI
{
    public class FileUploadState
    {
        public FileUploadState()
        {
            OnQueueChanged += async () => await ProcessQueue();
        }

        private void NotifyStateChanged() => OnStateChanged?.Invoke();
        public event Action OnStateChanged;

        private void NotifyUploadComplete(FileUpload upload) => OnUploadComplete?.Invoke(upload);
        public event Action<FileUpload> OnUploadComplete;

        private void NotifyQueueChanged() => OnQueueChanged?.Invoke();
        private event Action OnQueueChanged;

        public List<FileUpload> Uploads { get; set; } = new List<FileUpload>();
        public void DismissUpload(FileUpload fileUpload)
        {
            if (fileUpload.Status == UploadFileResult.InProgress)
            {
                fileUpload.CancellationTokenSource.Cancel();
            }
            else
            {
                fileUpload.Dispose();
                Uploads.Remove(fileUpload);
            }
            
            NotifyStateChanged();
        }
        public void DismissAll()
        {
            foreach (var upload in Uploads)
            {
                upload.Dispose();
            }

            Uploads.Clear();

            NotifyStateChanged();
        }

        public void Enqueue(FileUpload fileUpload)
        {
            fileUpload.Status = UploadFileResult.Queued;
            fileUpload.OnProgressChanged += NotifyStateChanged;

            int index = Uploads.FindIndex(x => !(x.Status == UploadFileResult.InProgress || x.Status == UploadFileResult.Queued));
            index = index < 0 ? Uploads.Count : index;

            Uploads.Insert(index, fileUpload);

            NotifyQueueChanged();

            NotifyStateChanged();
        }

        public async Task ProcessQueue()
        {
            for (int i = 0; i < Uploads.Count; i++)
            {
                switch (Uploads.First().Status)
                {
                    case UploadFileResult.Success:
                    case UploadFileResult.Cancelled:
                    case UploadFileResult.Failed:

                        if (Uploads.Any(x => x.Status == UploadFileResult.InProgress || x.Status == UploadFileResult.Queued))
                        {
                            FileUpload upload = Uploads.First();

                            Uploads.Remove(upload);
                            Uploads.Add(upload);
                        }

                        NotifyStateChanged();

                        break;
                    case UploadFileResult.Queued:
                        await UploadFile(Uploads.First());
                        break;

                    case UploadFileResult.InProgress:
                        return;
                }
            }
        }

        private async Task UploadFile(FileUpload fileUpload)
        {
            if (!Uploads.Any())
            {
                return;
            }

            if (fileUpload.Status != UploadFileResult.Queued)
            {
                return;
            }

            fileUpload.Status = UploadFileResult.InProgress;

            NotifyStateChanged();

            fileUpload.Status = await fileUpload.Task.Invoke(fileUpload.UploadStream);

            if (fileUpload.Status == UploadFileResult.Success)
            {
                NotifyUploadComplete(fileUpload);
            }

            await ProcessQueue();

            NotifyStateChanged();
        }
    }
}
