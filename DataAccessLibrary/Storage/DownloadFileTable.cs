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

        public async Task<List<DownloadFileModel>> Select(string downloadid)
        {
            string sql = "SELECT * FROM `downloadfiles` WHERE `DownloadId` = @downloadid";

            object parameters = new
            {
                downloadid
            };

            return await _sqlDataAccess.LoadData<DownloadFileModel>(sql, parameters);
        }

        public async Task Insert(DownloadFileModel file)
        {
            string sql = "INSERT INTO `downloadfiles` (DownloadId, Filename) VALUES (@DownloadId, @Filename)";

            await _sqlDataAccess.SaveData(sql, file);
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

        public async Task DeleteByFile(DownloadFileModel file)
        {
            string sql = "DELETE FROM `downloadfiles` WHERE `DownloadId` = @DownloadId AND `Filename` = @Filename";

            await _sqlDataAccess.SaveData(sql, file);
        }
    }
}
