using DataAccessLibrary.DataAccess;
using DataAccessLibrary.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Storage
{
    public class DownloadAuthorTable : IDownloadAuthorTable
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public DownloadAuthorTable(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<List<DownloadAuthorModel>> SelectByDownload(string downloadid)
        {
            string sql = "SELECT * FROM `downloadauthors` WHERE `DownloadId` = @downloadid";

            object parameters = new
            {
                downloadid
            };

            return await _sqlDataAccess.LoadData<DownloadAuthorModel>(sql, parameters);
        }

        public async Task<List<DownloadAuthorModel>> SelectByUser(string userid)
        {
            string sql = "SELECT * FROM `downloadauthors` WHERE `UserId` = @userid";

            object parameters = new
            {
                userid
            };

            return await _sqlDataAccess.LoadData<DownloadAuthorModel>(sql, parameters);
        }

        public async Task Insert(DownloadAuthorModel author)
        {
            string sql = "INSERT INTO `downloadauthors` (DownloadId, UserId) VALUES (@DownloadId, @UserId)";

            await _sqlDataAccess.SaveData(sql, author);
        }

        public async Task Delete(DownloadAuthorModel author)
        {
            string sql = "DELETE FROM `downloadauthors` WHERE `DownloadId` = @DownloadId AND `UserId` = @UserId";

            await _sqlDataAccess.SaveData(sql, author);
        }

        public async Task DeleteByDownload(string downloadid)
        {
            string sql = "DELETE FROM `downloadauthors` WHERE `DownloadId` = @downloadid";

            object parameters = new
            {
                downloadid
            };

            await _sqlDataAccess.SaveData(sql, downloadid);
        }

        public async Task DeleteByUser(string userid)
        {
            string sql = "DELETE FROM `downloadauthors` WHERE `UserId` = @UserId";

            object parameters = new
            {
                userid
            };

            await _sqlDataAccess.SaveData(sql, userid);
        }
    }
}
