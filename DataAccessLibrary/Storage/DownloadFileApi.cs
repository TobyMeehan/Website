using DataAccessLibrary.DataAccess;
using DataAccessLibrary.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Storage
{
    public class DownloadFileApi : IDownloadFileApi
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpDataAccess _httpDataAccess;

        public DownloadFileApi(IConfiguration configuration, IHttpDataAccess httpDataAccess)
        {
            _configuration = configuration;
            _httpDataAccess = httpDataAccess;
        }

        public async Task Post(DownloadFileModel file, MemoryStream stream)
        {
            string downloadHost = _configuration.GetSection("DownloadHost").Value;
            string uri = $"{downloadHost}/upload/{file.DownloadId}";

            MultipartFormDataContent content = new MultipartFormDataContent
            {
                { new ByteArrayContent(stream.GetBuffer()), "\"file\"", file.Filename }
            };

            await _httpDataAccess.PostHttpContent(uri, content);
        }

        public async Task Delete(string downloadid, string filename)
        {
            string downloadHost = _configuration.GetSection("DownloadHost").Value;
            string uri = $"{downloadHost}/{downloadid}/{filename}";
            await _httpDataAccess.Delete(uri);
        }
    }
}
