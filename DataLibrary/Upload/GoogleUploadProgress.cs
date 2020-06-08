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
            PercentageProgress = (int)((decimal)googleUploadProgress.BytesSent / (decimal)totalBytes * 100m);
        }

        public UploadStatus Status { get; }
        public int PercentageProgress { get; }
    }
}
