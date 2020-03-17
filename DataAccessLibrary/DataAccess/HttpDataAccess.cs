using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.DataAccess
{
    public class HttpDataAccess : IHttpDataAccess
    {
        private readonly HttpClient _client;

        public HttpDataAccess(HttpClient client)
        {
            _client = client;
        }

        public async Task<T> Get<T>(string uri)
        {
            var response = await _client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<T>();
            }
            return default;
        }

        public async Task<string> Post<T>(string uri, T value)
        {
            var response = await _client.PostAsJsonAsync(uri, value);
            response.EnsureSuccessStatusCode();
            return response.Headers.Location.ToString();
        }

        public async Task<string> PostHttpContent(string uri, HttpContent value)
        {
            var response = await _client.PostAsync(uri, value);
            response.EnsureSuccessStatusCode();
            return response.Headers.Location.ToString();
        }

        public async Task<T> PostThenGet<T>(string uri, T value)
        {
            string location = await Post(uri, value);
            return await Get<T>(location);
        }

        public async Task<T> Put<T>(string uri, T value)
        {
            var response = await _client.PutAsJsonAsync(uri, value);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<T>();
        }

        public async Task<HttpStatusCode> Delete(string uri)
        {
            var response = await _client.DeleteAsync(uri);
            return response.StatusCode;
        }
    }
}
