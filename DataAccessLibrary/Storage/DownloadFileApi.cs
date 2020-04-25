using DataAccessLibrary.DataAccess;
using DataAccessLibrary.Extensions;
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

        public async Task Post(DownloadFileModel file, Stream stream, int bufferSize, IProgress<int> progress) // TODO: cancel file upload
        {
            string downloadHost = _configuration.GetSection("DownloadHost").Value;
            string uri = $"{downloadHost}/upload/{file.DownloadId}";

            int totalFiles = (int)Math.Ceiling((decimal)stream.Length / (decimal)bufferSize);
            int processedFiles = 1;
            byte[] buffer = new byte[bufferSize];
            
            while (stream.Position < stream.Length)
            {
                int length = Math.Min(bufferSize, (int)(stream.Length - stream.Position));

                await stream.ReadAsync(buffer, 0, length);

                using (MultipartFormDataContent content = new MultipartFormDataContent())
                {
                    using (ByteArrayContent contents = new ByteArrayContent(buffer, 0, length))
                    {
                        content.Add(contents, "\"file\"", $"{file.Filename}.part.{processedFiles}");

                        var response = await _client.PostAsync(uri, content);

                        if (response.IsSuccessStatusCode)
                        {
                            int percentageProgress = (int)(((decimal)processedFiles / (decimal)totalFiles) * 100m);
                            progress.Report(percentageProgress);
                        }
                        else
                        {
                            // TODO: cancel if failed
                        }
                    }
                }

                GC.Collect(); // TODO: remove asap

                processedFiles++;

                Thread.Sleep(10);
            }
        }

        public async Task Delete(string downloadid, string filename)
        {
            string downloadHost = _configuration.GetSection("DownloadHost").Value;
            string uri = $"{downloadHost}/{downloadid}/{filename}";
            await _httpDataAccess.Delete(uri);
        }
    }
}
