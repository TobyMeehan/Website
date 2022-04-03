using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Upload;

namespace TobyMeehan.Com.Data.CloudStorage
{
    public interface ICloudStorage
    {
        Task DeleteFileAsync(string bucket, string objectName);
        Task<IFile> UploadFileAsync(Stream stream, string bucket, string objectName, string filename, MediaType contentType, CancellationToken cancellationToken = default, IProgress<IUploadProgress> progress = null);
        Task<IFile> GetFileAsync(string bucket, string objectName);
        Task DownloadFileAsync(string bucket, string objectName, Stream destination);
        Task RenameFileAsync(string bucket, string objectName, string filename);
    }
}