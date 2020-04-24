using DataAccessLibrary.Models;
using System.IO;
using System.Threading.Tasks;

namespace DataAccessLibrary.Storage
{
    public interface IDownloadFileApi
    {
        Task Delete(string downloadid, string filename);
        Task Post(DownloadFileModel file, Stream stream);
    }
}