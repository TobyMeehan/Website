using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Models;

namespace TobyMeehan.Com.Data.Repositories
{
    public interface IDownloadRepository
    {
        Task<IList<Download>> GetAsync();

        Task<IList<Download>> GetByAuthorAsync(string userId);

        Task<Download> GetByIdAsync(string id);

        Task<Download> AddAsync(string title, string shortDescription, string longDescription);

        Task UpdateAsync(Download download);

        Task AddAuthorAsync(string id, string userId);

        Task AddFileAsync(string id, string filename, Stream uploadStream);

        Task DeleteAsync(string id);
    }
}
