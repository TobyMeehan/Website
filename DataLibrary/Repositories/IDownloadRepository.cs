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
    public interface IDownloadRepository
    {
        Task<IList<Download>> GetAsync();

        Task<IList<Download>> GetByAuthorAsync(string userId);

        Task<Download> GetByIdAsync(string id);

        Task<Download> AddAsync(string title, string shortDescription, string longDescription, string userId);

        Task UpdateAsync(Download download);

        Task AddAuthorAsync(string id, string userId);

        Task RemoveAuthorAsync(string id, string userId);

        Task DeleteAsync(string id);
    }
}
