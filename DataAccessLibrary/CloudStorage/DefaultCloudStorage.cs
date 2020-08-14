using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Upload;

namespace TobyMeehan.Com.Data.CloudStorage
{
    public class DefaultCloudStorage : ICloudStorage
    {
        public Task DeleteFileAsync(string bucket, string filename)
        {
            throw new NotImplementedException();
        }

        public Task DownloadFileAsync(string bucket, string filename, Stream destination)
        {
            throw new NotImplementedException();
        }

        public Task<CloudFile> UploadFileAsync(Stream stream, string bucket, string objectName, string filename, string contentType, CancellationToken cancellationToken = default, IProgress<IUploadProgress> progress = null)
        {
            throw new NotImplementedException();
        }
    }
}
