using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Collections;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Upload;

namespace TobyMeehan.Com.Data.Repositories
{
    public interface IDownloadFileRepository
    {
        Task<IEntityCollection<DownloadFile>> GetAsync();

        Task<IEntityCollection<DownloadFile>> GetByFilenameAsync(string filename);

        Task<IEntityCollection<DownloadFile>> GetByDownloadAsync(string downloadId);

        Task<IEntityCollection<DownloadFile>> GetByDownloadAndFilenameAsync(string downloadId, string filename);

        Task<DownloadFile> GetByIdAsync(string id);

        Task DownloadAsync(string id, Stream stream);

        Task<DownloadFile> AddAsync(string downloadId, string filename, Stream uploadStream, CancellationToken cancellationToken = default, IProgress<IUploadProgress> progress = null);

        Task UpdateFilenameAsync(string id, string filename);

        Task DeleteAsync(string id);
    }
}
