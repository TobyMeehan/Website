using DataAccessLibrary.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLibrary.Storage
{
    public interface IDownloadTable
    {
        Task Delete(string downloadid);
        Task DeleteByUser(string userid);
        Task Insert(Download download);
        Task<List<Download>> Search(string query);
        Task<List<Download>> Select();
        Task<List<Download>> SelectById(string downloadid);
        Task<List<Download>> SelectByUser(string userid);
        Task Update(Download download);
        Task UpdateVerified(string downloadid, DownloadVerification verified);
    }
}