using DataAccessLibrary.DataAccess;
using DataAccessLibrary.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLibrary.Storage
{
    public class DownloadFileTable : IDownloadFileTable
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public DownloadFileTable(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<List<DownloadFile>> SelectByDownload(string downloadid)
        {
            string sql = "SELECT * FROM `downloadfiles` WHERE `DownloadId` = @downloadid";

            object parameters = new
            {
                downloadid
            };

            return await _sqlDataAccess.LoadData<DownloadFile>(sql, parameters);
        }

        public async Task<List<DownloadFile>> SelectById(string id)
        {
            string sql = "SELECT * FROM `downloadfiles` WHERE `Id` = @id";

            object parameters = new
            {
                id
            };

            return await _sqlDataAccess.LoadData<DownloadFile>(sql, parameters);
        }

        public async Task Insert(string downloadId, string filename, string randomName)
        {
            string sql = "INSERT INTO `downloadfiles` (Id, DownloadId, Filename, RandomName) VALUES (UUID(), @downloadId, @filename, @randomName)";

            object parameters = new
            {
                downloadId,
                filename,
                randomName
            };

            await _sqlDataAccess.SaveData(sql, parameters);
        }

        public async Task DeleteByDownload(string downloadid)
        {
            string sql = "DELETE FROM `downloadfiles` WHERE `DownloadId` = @downloadid";

            object parameters = new
            {
                downloadid
            };

            await _sqlDataAccess.SaveData(sql, parameters);
        }

        public async Task DeleteByFile(string id)
        {
            string sql = "DELETE FROM `downloadfiles` WHERE `Id` = @id";

            object parameters = new
            {
                id
            };

            await _sqlDataAccess.SaveData(sql, parameters);
        }
    }
}
