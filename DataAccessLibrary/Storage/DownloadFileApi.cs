using DataAccessLibrary.DataAccess;
using DataAccessLibrary.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TobyMeehan.Http;

namespace DataAccessLibrary.Storage
{
    public class DownloadFileApi : IDownloadFileApi
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpDataAccess _httpDataAccess;
        private readonly HttpClient _client;

        public DownloadFileApi(IConfiguration configuration, IHttpDataAccess httpDataAccess, HttpClient client)
        {
            _configuration = configuration;
            _httpDataAccess = httpDataAccess;
            _client = client;
        }

        public async Task<bool> Post(DownloadFileModel file, Stream stream, int bufferSize, IProgress<int> progress, CancellationToken cancellationToken)
        {
            string downloadHost = _configuration.GetSection("DownloadHost").Value;
            string secret = _configuration.GetSection("UploadSecret").Value;

            string uri = $"{downloadHost}/token";
            bool result = false;

            await _client.Post(uri, new
            {
                secret,
                file.DownloadId,
                file.Filename
            })
                .OnOK<dynamic>((response) =>
                {
                    string token = response.token;
                    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    result = true;
                })
                .OnBadRequest(() =>
                {
                    result = false;
                })
                .SendAsync();


            if (!result) return result;

            uri = $"{downloadHost}/upload/{file.DownloadId}";

            int totalFiles = (int)Math.Ceiling((decimal)stream.Length / (decimal)bufferSize);
            int processedFiles = 1;
            byte[] buffer = new byte[bufferSize];

            while (stream.Position < stream.Length)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    _client.DefaultRequestHeaders.Clear();
                    cancellationToken.ThrowIfCancellationRequested();
                }

                cancellationToken.ThrowIfCancellationRequested();

                int length = Math.Min(bufferSize, (int)(stream.Length - stream.Position));

                await stream.ReadAsync(buffer, 0, length);

                using (MultipartFormDataContent content = new MultipartFormDataContent())
                {
                    using (ByteArrayContent contents = new ByteArrayContent(buffer, 0, length))
                    {
                        content.Add(contents, "\"file\"", $"{file.Filename}.part.{processedFiles}");

                        await _client.PostHttpContent(uri, content)
                            .OnOK(() =>
                            {
                                int percentageProgress = (int)(((decimal)processedFiles / (decimal)totalFiles) * 100m);
                                progress.Report(percentageProgress);
                                result = true;
                            })
                            .OnBadRequest((statusCode) =>
                            {
                                result = false;
                            })
                            .SendAsync();

                        if (!result) return result;
                    }
                }

                GC.Collect(); // TODO: remove asap

                processedFiles++;

                Thread.Sleep(10);
            }

            return true;
        }

        public async Task Delete(string downloadid, string filename)
        {
            string downloadHost = _configuration.GetSection("DownloadHost").Value;
            string uri = $"{downloadHost}/{downloadid}/{filename}";
            await _httpDataAccess.Delete(uri);
        }
    }
}
