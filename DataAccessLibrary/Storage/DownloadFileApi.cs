using DataAccessLibrary.DataAccess;
using DataAccessLibrary.Models;
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
        const string HostUri = "localhost:44368";

        private readonly IHttpDataAccess _httpDataAccess;

        public DownloadFileApi(IHttpDataAccess httpDataAccess)
        {
            _httpDataAccess = httpDataAccess;
        }

        public async Task Post(DownloadFileModel file, byte[] contents)
        {
            string uri = $"https://{HostUri}/upload/{file.DownloadId}/{file.Filename}";
            await _httpDataAccess.Post(uri, Convert.ToBase64String(contents));
        }

        public async Task Delete(string downloadid, string filename)
        {
            string uri = $"https://{HostUri}/{downloadid}/{filename}";
            await _httpDataAccess.Delete(uri);
        }
    }
}
