using DataAccessLibrary.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLibrary.Storage
{
    public interface IDownloadFileTable
    {
        Task DeleteByDownload(string downloadid);
        Task DeleteByFile(string id);
        Task Insert(string downloadId, string filename, string randomName);
        Task<List<DownloadFile>> SelectByDownload(string downloadid);
        Task<List<DownloadFile>> SelectById(string id);
    }
}