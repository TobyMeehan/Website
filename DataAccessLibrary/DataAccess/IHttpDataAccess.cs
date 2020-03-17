using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace DataAccessLibrary.DataAccess
{
    public interface IHttpDataAccess
    {
        Task<HttpStatusCode> Delete(string uri);
        Task<T> Get<T>(string uri);
        Task<string> Post<T>(string uri, T value);
        Task<string> PostHttpContent(string uri, HttpContent value);
        Task<T> PostThenGet<T>(string uri, T value);
        Task<T> Put<T>(string uri, T value);
    }
}