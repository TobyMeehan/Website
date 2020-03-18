using DataAccessLibrary.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLibrary.Storage
{
    public interface IDownloadAuthorTable
    {
        Task Delete(DownloadAuthorModel author);
        Task DeleteByDownload(string downloadid);
        Task DeleteByUser(string userid);
        Task Insert(DownloadAuthorModel author);
        Task<List<DownloadAuthorModel>> SelectByDownload(string downloadid);
        Task<List<DownloadAuthorModel>> SelectByUser(string userid);
    }
}