using DataAccessLibrary.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLibrary.Data
{
    public interface IDownloadProcessor
    {
        Task CreateDownload(Download download);
        Task DeleteDownload(string downloadid);
        Task<Download> GetDownloadById(string downloadid);
        Task<List<Download>> GetDownloads();
        Task<List<Download>> GetDownloadsByAuthor(string userid);
        Task<List<Download>> GetDownloadsByCreator(string userid);
        Task<List<Download>> SearchDownloads(string query);
        Task UpdateDownload(Download download);
    }
}