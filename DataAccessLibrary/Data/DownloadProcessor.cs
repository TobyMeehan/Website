using DataAccessLibrary.Models;
using DataAccessLibrary.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Data
{
    public class DownloadProcessor : ProcessorBase, IDownloadProcessor
    {
        private readonly IDownloadTable _downloadTable;
        private readonly IDownloadAuthorTable _downloadAuthorTable;
        private readonly IUserTable _userTable;

        public DownloadProcessor(IDownloadTable downloadTable, IDownloadAuthorTable downloadAuthorTable, IUserTable userTable)
        {
            _downloadTable = downloadTable;
            _downloadAuthorTable = downloadAuthorTable;
            _userTable = userTable;
        }

        private async Task<Download> PopulateAuthor(Download download)
        {
            if (ValidateQuery(await _userTable.SelectById(download.CreatorId), out User creator))
            {
                download.CreatorId = creator.Id;
                download.Authors.Add(creator);
            }
            else
            {
                throw new Exception("Provided creator id could not be found.");
            }

            var downloadAuthors = await _downloadAuthorTable.SelectByDownload(download.Id);

            foreach (DownloadAuthorModel downloadAuthor in downloadAuthors)
            {
                if (ValidateQuery(await _userTable.SelectById(downloadAuthor.UserId), out User user))
                {
                    download.Authors.Add(user);
                }
            }

            return download;
        }

        private async Task<List<Download>> PopulateAuthor(List<Download> downloads)
        {
            foreach (Download download in downloads)
            {
                await PopulateAuthor(download);
            }

            return downloads;
        }

        public async Task<List<Download>> GetDownloads()
        {
            List<Download> downloads = await _downloadTable.Select();

            return await PopulateAuthor(downloads);
        }

        public async Task<List<Download>> GetDownloadsByCreator(string userid)
        {
            List<Download> downloads = await _downloadTable.SelectByUser(userid);

            return await PopulateAuthor(downloads);
        }

        public async Task<List<Download>> GetDownloadsByAuthor(string userid)
        {
            List<Download> downloads = await _downloadTable.SelectByUser(userid);
            List<DownloadAuthorModel> downloadAuthors = await _downloadAuthorTable.SelectByUser(userid);

            foreach (DownloadAuthorModel downloadAuthor in downloadAuthors)
            {
                if (ValidateQuery(await _downloadTable.SelectById(downloadAuthor.DownloadId), out Download download))
                {
                    downloads.Add(download);
                }
            }

            return await PopulateAuthor(downloads);
        }

        public async Task<Download> GetDownloadById(string downloadid)
        {
            if (ValidateQuery(await _downloadTable.SelectById(downloadid), out Download download))
            {
                return await PopulateAuthor(download);
            }
            else
            {
                return null;
            }
        }

        public async Task<List<Download>> SearchDownloads(string query)
        {
            List<Download> downloads = await _downloadTable.Search(query);

            return await PopulateAuthor(downloads);
        }

        public async Task CreateDownload(Download download)
        {
            download.Updated = DateTime.Now;
            await _downloadTable.Insert(download);
        }

        public async Task UpdateDownload(Download download)
        {
            download.Updated = DateTime.Now;
            await _downloadTable.Update(download);
        }

        public async Task DeleteDownload(string downloadid)
        {
            await _downloadTable.Delete(downloadid);
        }
    }
}
