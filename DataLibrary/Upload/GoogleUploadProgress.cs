using System;
using System.Collections.Generic;
using System.Text;

namespace TobyMeehan.Com.Data.Upload
{
    public class GoogleUploadProgress : IUploadProgress
    {
        public GoogleUploadProgress(Google.Apis.Upload.IUploadProgress googleUploadProgress, long totalBytes)
        {
            Status = (UploadStatus)googleUploadProgress.Status;
            PercentageProgress = (int)(googleUploadProgress.BytesSent / totalBytes * 100);
        }

        public UploadStatus Status { get; }
        public int PercentageProgress { get; }
    }
}
