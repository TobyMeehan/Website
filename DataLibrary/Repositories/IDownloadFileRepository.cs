using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Upload;

namespace TobyMeehan.Com.Data.Repositories
{
    public interface IDownloadFileRepository
    {
        Task<IList<DownloadFile>> GetAsync();

        Task<IList<DownloadFile>> GetByFilenameAsync(string filename);

        Task<IList<DownloadFile>> GetByDownloadAsync(string downloadId);

        Task<DownloadFile> AddAsync(string downloadId, string filename, Stream uploadStream, CancellationToken cancellationToken = default, IProgress<IUploadProgress> progress = null);

        Task DeleteAsync(string id);
    }
}
