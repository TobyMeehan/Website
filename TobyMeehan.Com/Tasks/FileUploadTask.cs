using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Upload;

namespace TobyMeehan.Com.Tasks
{
    public class FileUploadTask : ProgressTaskBase
    {
        public FileUploadTask(string filename, Download download, Func<CancellationToken, Progress<IUploadProgress>, Task> task)
        {
            Progress<IUploadProgress> progress = new Progress<IUploadProgress>();

            progress.ProgressChanged += (s, e) =>
            {
                PercentageProgress = e.PercentageProgress;

                Status = e.Status switch
                {
                    UploadStatus.NotStarted => TaskStatus.InProgress,
                    UploadStatus.Starting => TaskStatus.InProgress,
                    UploadStatus.Uploading => TaskStatus.InProgress,
                    UploadStatus.Completed => TaskStatus.Completed,
                    UploadStatus.Failed => TaskStatus.Failed
                };

                NotifyProgressChanged();
            };

            TaskSource = ct => task(ct, progress);
            Filename = filename;
            Download = download;
        }

        public string Filename { get; set; }
        public Download Download { get; set; }
    }
}
