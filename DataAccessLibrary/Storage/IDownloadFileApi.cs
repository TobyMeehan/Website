using DataAccessLibrary.Models;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessLibrary.Storage
{
    public interface IDownloadFileApi
    {
        int GetTotalFiles(int length, int bufferSize);
        Task Delete(string downloadid, string filename);
        Task<UploadToken> PostToken(string downloadId, string filename, int partitions);
        Task<bool> PostFile(UploadToken token, Stream stream, int bufferSize, IProgress<int> progress, CancellationToken cancellationToken);
    }
}