using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Models;

namespace TobyMeehan.Com.Data.Extensions
{
    public static class DownloadRepositoryExtensions
    {
        public static Task<IList<Download>> GetByUser(this IRepository<Download> downloads, string id)
        {
            return downloads.GetByAsync<User>((d, u) => u.Id == id);
        }

        public static Task VerifyDownload(this IRepository<Download> downloads, string id, DownloadVerification verification)
        {
            return downloads.UpdateByIdAsync(id, new
            {
                Verified = verification
            });
        }
    }
}
