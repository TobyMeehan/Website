using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Models;

namespace TobyMeehan.Com.Data.Extensions
{
    public static class DownloadRepositoryExtensions
    {
        public static Task<IEnumerable<Download>> GetByUser(this IRepository<Download> downloads, string id)
        {
            return downloads.GetByAsync<User>((d, u) => u.Id == id);
        }
    }
}
