using DataAccessLibrary.DataAccess;
using DataAccessLibrary.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Storage
{
    public class DownloadTable : IDownloadTable
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public DownloadTable(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<List<Download>> Select()
        {
            string sql = "SELECT * FROM `downloads`";

            return await _sqlDataAccess.LoadData<Download>(sql);
        }

        public async Task<List<Download>> SelectById(string downloadid)
        {
            string sql = "SELECT * FROM `downloads` WHERE `Id` = @downloadid";

            object parameters = new
            {
                downloadid
            };

            return await _sqlDataAccess.LoadData<Download>(sql, parameters);
        }

        public async Task<List<Download>> SelectByUser(string userid)
        {
            string sql = "SELECT * FROM `downloads` WHERE `CreatorId` = @userid";

            object parameters = new
            {
                userid
            };

            return await _sqlDataAccess.LoadData<Download>(sql, parameters);
        }

        public async Task<List<Download>> Search(string query)
        {
            string sql = "SELECT * FROM `downloads` WHERE `Title` LIKE '%' + @query + '%'";

            object parameters = new
            {
                query
            };

            return await _sqlDataAccess.LoadData<Download>(sql, parameters);
        }

        public async Task Insert(Download download)
        {
            string sql = "INSERT INTO `downloads` (Id, CreatorId, Title, ShortDescription, LongDescription, Version, Updated) VALUES (UUID(), @CreatorId, @Title, @ShortDescription, @LongDescription, @Version, @Updated";

            await _sqlDataAccess.SaveData(sql, download);
        }

        public async Task Update(Download download)
        {
            string sql = $"UPDATE `downloads` SET " +
                $"{(download.Title != null ? "Title = @Title," : "")} " +
                $"{(download.ShortDescription != null ? "ShortDescription = @ShortDescription," : "")} " +
                $"{(download.LongDescription != null ? "LongDescription = @LongDescription," : "")} " +
                $"Updated = @Updated";

            await _sqlDataAccess.SaveData(sql, download);
        }

        public async Task Delete(string downloadid)
        {
            string sql = "DELETE FROM `downloads` WHERE `Id` = @downloadid";

            object parameters = new
            {
                downloadid
            };

            await _sqlDataAccess.SaveData(sql, parameters);
        }
    }
}
