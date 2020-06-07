using System;
using System.Collections.Generic;
using System.Text;

namespace TobyMeehan.Com.Data.Upload
{
    public interface IUploadProgress
    {
        UploadStatus Status { get; }
        int PercentageProgress { get; }
    }
}
