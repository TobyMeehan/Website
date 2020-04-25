using DataAccessLibrary.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessLibrary.Data
{
    public interface IDownloadProcessor
    {
        Task<Download> CreateDownload(Download download);
        Task<UploadFileResult> TryAddFile(DownloadFileModel file, Stream stream, int bufferSize, IProgress<int> progress, CancellationToken cancellationToken);
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
        Task<bool> IsAuthor(string downloadid, string userid);
    }
}