using DataAccessLibrary.DataAccess;
using DataAccessLibrary.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
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

        public async Task Post(DownloadFileModel file, byte[] contents)
        {
            string downloadHost = _configuration.GetSection("DownloadHost").Value;
            string uri = $"{downloadHost}/upload/{file.DownloadId}/{file.Filename}";
            await _httpDataAccess.Post(uri, Convert.ToBase64String(contents));
        }

        public async Task Delete(string downloadid, string filename)
        {
            string downloadHost = _configuration.GetSection("DownloadHost").Value;
            string uri = $"{downloadHost}/{downloadid}/{filename}";
            await _httpDataAccess.Delete(uri);
        }
    }
}
