using DataAccessLibrary.Models;
using System.Threading.Tasks;

namespace DataAccessLibrary.Storage
{
    public interface IDownloadFileApi
    {
        Task Delete(string downloadid, string filename);
        Task Post(DownloadFileModel file, byte[] contents);
    }
}