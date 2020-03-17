using DataAccessLibrary.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLibrary.Storage
{
    public interface IDownloadFileTable
    {
        Task Insert(DownloadFileModel file);
        Task DeleteByDownload(string downloadid);
        Task DeleteByFile(DownloadFileModel file);
        Task<List<DownloadFileModel>> Select(string downloadid);
    }
}