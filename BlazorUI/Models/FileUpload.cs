using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorUI.Models
{
    public class FileUpload
    {
        public FileUpload(string filename, string download, Task<UploadFileResult> task, Progress<int> progress, CancellationTokenSource cts)
        {
            Filename = filename;
            Download = download;
            Task = task;

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
        public Task<UploadFileResult> Task { get; set; }
        public UploadFileResult Status { get; set; } = UploadFileResult.InProgress;
        public IProgress<int> Progress { get; set; }
        public int PercentageProgress { get; set; }
        public CancellationTokenSource CancellationTokenSource { get; set; }

        public event Action OnProgressChanged;
    }
}
