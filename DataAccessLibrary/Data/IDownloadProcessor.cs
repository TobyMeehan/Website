using DataAccessLibrary.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLibrary.Data
{
    public interface IDownloadProcessor
    {
        Task<Download> CreateDownload(Download download);
        Task CreateFile(DownloadFileModel file, byte[] contents);
        Task DeleteDownload(string downloadid);
        Task DeleteFile(DownloadFileModel file);
        Task<Download> GetDownloadById(string downloadid);
        Task<List<Download>> GetDownloads();
        Task<List<Download>> GetDownloadsByAuthor(string userid);
        Task<List<Download>> GetDownloadsByCreator(string userid);
        Task<List<Download>> SearchDownloads(string query);
        Task AddAuthor(string downloadid, string userid);
        Task RemoveAuthor(string downloadid, string userid);
        Task UpdateDownload(Download download);
        Task VerifyDownload(string downloadid, DownloadVerification verified);
    }
}