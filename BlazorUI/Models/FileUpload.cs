using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorUI.Models
{
    public class FileUpload
    {
        public FileUpload(string filename, string download, Task<bool> task, Progress<int> progress)
        {
            Filename = filename;
            Download = download;
            Task = task;

            progress.ProgressChanged += Progress_ProgressChanged;
            Progress = progress;
        }

        private void Progress_ProgressChanged(object sender, int e)
        {
            PercentageProgress = e;
            OnProgressChanged?.Invoke();
        }

        public string Filename { get; set; }
        public string Download { get; set; }
        public Task<bool> Task { get; set; }
        public IProgress<int> Progress { get; set; }
        public int PercentageProgress { get; set; }
        public CancellationToken CancellationToken { get; set; }

        public event Action OnProgressChanged;
    }
}
