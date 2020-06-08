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
        private readonly Stream _uploadStream;

        public FileUploadTask(string filename, Download download, Stream uploadStream, Func<Stream, CancellationToken, Progress<IUploadProgress>, Task> task)
        {
            Progress<IUploadProgress> progress = new Progress<IUploadProgress>();

            progress.ProgressChanged += (s, e) =>
            {
                PercentageProgress = e.PercentageProgress;

                NotifyProgressChanged();
            };

            TaskSource = ct => task(_uploadStream, ct, progress);
            Filename = filename;
            Download = download;
            _uploadStream = uploadStream;
        }

        public string Filename { get; set; }
        public Download Download { get; set; }

        public override void Dispose()
        {
            _uploadStream.Dispose();

            base.Dispose();
        }
    }
}
