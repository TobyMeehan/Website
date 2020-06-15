using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Upload;
using System.Net.Mime;
using Google.Apis.Storage.v1.Data;

namespace TobyMeehan.Com.Data.CloudStorage
{
    public class GoogleCloudStorage : ICloudStorage
    {
        private readonly GoogleCredential _credential;
        private readonly int _chunkSize = 512 * 1024;

        public GoogleCloudStorage(GoogleCredential credential)
        {
            _credential = credential;
        }

        private string GetDownloadLink(Google.Apis.Storage.v1.Data.Object obj)
        {
            return $"https://storage.googleapis.com/{obj.Bucket}/{obj.Name}";
        }

        public async Task DeleteFileAsync(string bucket, string filename)
        {
            using (StorageClient client = await StorageClient.CreateAsync(_credential))
            {
                await client.DeleteObjectAsync(bucket, filename);
            }
        }

        public async Task<CloudFile> UploadFileAsync(Stream stream, string bucket, string objectName, string filename, CancellationToken cancellationToken = default, IProgress<IUploadProgress> progress = null)
        {
            Progress<Google.Apis.Upload.IUploadProgress> googleProgress = new Progress<Google.Apis.Upload.IUploadProgress>();

            if (progress != null)
            {
                googleProgress.ProgressChanged += (s, e) =>
                {
                    progress.Report(new GoogleUploadProgress(e, stream.Length));
                    GC.Collect();
                };
            }

            using (StorageClient client = await StorageClient.CreateAsync(_credential))
            {
                UploadObjectOptions options = new UploadObjectOptions
                {
                    ChunkSize = _chunkSize
                };

                var dataObject = await client.UploadObjectAsync(bucket, objectName, MediaTypeNames.Application.Octet, stream, options, cancellationToken, googleProgress);

                dataObject.ContentDisposition = $"filename=\"{filename}\"";

                dataObject = await client.PatchObjectAsync(dataObject);

                return new CloudFile(GetDownloadLink(dataObject), dataObject.MediaLink);
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
