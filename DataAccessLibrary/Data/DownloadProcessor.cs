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
        private readonly IDownloadFileTable _downloadFileTable;
        private readonly IDownloadFileApi _downloadFileApi;
        private readonly IUserTable _userTable;

        public DownloadProcessor(IDownloadTable downloadTable, IDownloadAuthorTable downloadAuthorTable, IDownloadFileTable downloadFileTable, IDownloadFileApi downloadFileApi, IUserTable userTable)
        {
            _downloadTable = downloadTable;
            _downloadAuthorTable = downloadAuthorTable;
            _downloadFileTable = downloadFileTable;
            _downloadFileApi = downloadFileApi;
            _userTable = userTable;
        }

        private async Task<Download> PopulateAuthor(Download download)
        {
            if (ValidateQuery(await _userTable.SelectById(download.CreatorId), out User creator))
            {
                download.CreatorId = creator.Id;
                download.Authors = new List<User>
                {
                    creator
                };
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
        private async Task<Download> PopulateFiles(Download download)
        {
            var files = await _downloadFileTable.Select(download.Id);
            download.Files = new List<string>();

            foreach (DownloadFileModel file in files)
            {
                download.Files.Add(file.Filename);
            }

            return download;
        }
        private async Task<List<Download>> PopulateFiles(List<Download> downloads)
        {
            foreach (Download download in downloads)
            {
                await PopulateFiles(download);
            }

            return downloads;
        }
        private async Task<Download> Populate(Download download)
        {
            download = await PopulateFiles(await PopulateAuthor(download));
            return download;
        }
        private async Task<List<Download>> Populate(List<Download> downloads)
        {
            foreach (Download download in downloads)
            {
                await Populate(download);
            }

            return downloads.OrderBy(download => download.Updated).ThenBy(download => download.Title).ToList();
        }


        public async Task<List<Download>> GetDownloads()
        {
            List<Download> downloads = await _downloadTable.Select();

            return await Populate(downloads);
        }

        public async Task<List<Download>> GetDownloadsByCreator(string userid)
        {
            List<Download> downloads = await _downloadTable.SelectByUser(userid);

            return await Populate(downloads);
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

            return await Populate(downloads);
        }

        public async Task<Download> GetDownloadById(string downloadid)
        {
            if (ValidateQuery(await _downloadTable.SelectById(downloadid), out Download download))
            {
                return await Populate(download);
            }
            else
            {
                return null;
            }
        }

        public async Task<List<Download>> SearchDownloads(string query)
        {
            List<Download> downloads = await _downloadTable.Search(query);

            return await Populate(downloads);
        }

        public async Task<Download> CreateDownload(Download download)
        {
            download.Updated = DateTime.Now;
            download.Id = Security.RandomString.GeneratePseudo();
            await _downloadTable.Insert(download);
            return await GetDownloadById(download.Id);
        }

        public async Task CreateFile(DownloadFileModel file, byte[] contents)
        {
            if ((await _downloadTable.SelectById(file.DownloadId)).Any())
            {
                await _downloadFileTable.Insert(file);
                await _downloadFileApi.Post(file, contents);
            }
        }

        public async Task UpdateDownload(Download download)
        {
            download.Updated = DateTime.Now;
            await _downloadTable.Update(download);
        }

        public async Task VerifyDownload(string downloadid, DownloadVerification verified)
        {
            await _downloadTable.UpdateVerified(downloadid, verified);
        }

        public async Task AddAuthor(string downloadid, string userid)
        {
            if ((await _downloadTable.SelectById(downloadid)).Any())
            {
                if ((await _userTable.SelectById(userid)).Any())
                {
                    DownloadAuthorModel author = new DownloadAuthorModel { DownloadId = downloadid, UserId = userid };
                    await _downloadAuthorTable.Insert(author);
                }
                else
                {
                    throw new ArgumentException("No user with the provided ID could be found.");
                }
            }
        }

        public async Task RemoveAuthor(string downloadid, string userid)
        {
            if ((await _downloadTable.SelectById(downloadid)).Any())
            {
                if ((await _userTable.SelectById(userid)).Any())
                {
                    DownloadAuthorModel author = new DownloadAuthorModel { DownloadId = downloadid, UserId = userid };
                    await _downloadAuthorTable.Delete(author);
                }
                else
                {
                    throw new ArgumentException("No user with the provided ID could be found.");
                }
            }
        }

        public async Task DeleteDownload(string downloadid)
        {
            await _downloadTable.Delete(downloadid);
        }

        public async Task DeleteFile(DownloadFileModel file)
        {
            if ((await _downloadTable.SelectById(file.DownloadId)).Any())
            {
                if ((await _downloadFileTable.Select(file.DownloadId)).Where(f => f.Filename == file.Filename).Any())
                {
                    await _downloadFileTable.DeleteByFile(file);
                    await _downloadFileApi.Delete(file.DownloadId, file.Filename);
                }
            }
        }
    }
}
