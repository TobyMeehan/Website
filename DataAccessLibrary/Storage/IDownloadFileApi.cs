using DataAccessLibrary.Models;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessLibrary.Storage
{
    public interface IDownloadFileApi
    {
        Task Delete(string downloadid, string filename);
        Task<bool> Post(DownloadFileModel file, Stream stream, int bufferSize, IProgress<int> progress, CancellationToken cancellationToken);
    }
}