using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
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

        public async Task<string> UploadFileAsync(Stream stream, string bucket, string filename)
        {
            using (StorageClient client = await StorageClient.CreateAsync(_credential))
            {
                var dataObject = await client.UploadObjectAsync(bucket, filename, Application.Octet, stream);
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
