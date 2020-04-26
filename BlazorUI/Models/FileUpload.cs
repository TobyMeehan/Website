using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorUI.Models
{
    public class FileUpload : IDisposable
    {
        public FileUpload(string filename, string download, Func<Stream, Task<UploadFileResult>> task, Stream uploadStream, Progress<int> progress, CancellationTokenSource cts)
        {
            Filename = filename;
            Download = download;

            Task = task;
            UploadStream = uploadStream;

            progress.ProgressChanged += Progress_ProgressChanged;
            Progress = progress;

            CancellationTokenSource = cts;
        }

        private void Progress_ProgressChanged(object sender, int e)
        {
            PercentageProgress = e;
            OnProgressChanged?.Invoke();
        }

        public string Filename { get; set; }
        public string Download { get; set; }
        public Func<Stream, Task<UploadFileResult>> Task { get; set; }
        public Stream UploadStream { get; set; }
        public UploadFileResult Status { get; set; } = UploadFileResult.InProgress;
        public bool IsComplete => Status == UploadFileResult.Success ||
            Status == UploadFileResult.Cancelled ||
            Status == UploadFileResult.Failed;

        public IProgress<int> Progress { get; set; }
        public int PercentageProgress { get; set; }
        public CancellationTokenSource CancellationTokenSource { get; set; }

        public event Action OnProgressChanged;

        public void Dispose()
        {
            UploadStream?.Dispose();
            CancellationTokenSource?.Dispose();
        }
    }
}
