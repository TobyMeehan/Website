using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Upload;
using static System.Net.Mime.MediaTypeNames;

namespace TobyMeehan.Com.Data.CloudStorage
{
    public class GoogleCloudStorage : ICloudStorage
    {
        private readonly GoogleCredential _credential;

        public GoogleCloudStorage(GoogleCredential credential)
        {
            _credential = credential;
        }

        public async Task DeleteFileAsync(string bucket, string filename)
        {
            using (StorageClient client = await StorageClient.CreateAsync(_credential))
            {
                await client.DeleteObjectAsync(bucket, filename);
            }
        }

        public async Task<string> UploadFileAsync(Stream stream, string bucket, string filename, CancellationToken cancellationToken = default, IProgress<IUploadProgress> progress = null)
        {
            Progress<Google.Apis.Upload.IUploadProgress> googleProgress = new Progress<Google.Apis.Upload.IUploadProgress>();

            if (progress != null)
            {
                googleProgress.ProgressChanged += (s, e) =>
                {
                    progress.Report(new GoogleUploadProgress(e, stream.Length));
                };
            }

            using (StorageClient client = await StorageClient.CreateAsync(_credential))
            {
                var dataObject = await client.UploadObjectAsync(bucket, filename, Application.Octet, stream, cancellationToken: cancellationToken, progress: googleProgress);
                return dataObject.MediaLink;
            }
        }

        public async Task DownloadFileAsync(string bucket, string filename, Stream destination)
        {
            using (StorageClient client = await StorageClient.CreateAsync(_credential))
            {
                await client.DownloadObjectAsync(bucket, filename, destination);
            }
        }
    }
}
