using DataAccessLibrary.DataAccess;
using DataAccessLibrary.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Storage
{
    public class PkceTable : IPkceTable
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public PkceTable(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<List<Pkce>> Select(string clientid)
        {
            string sql = "SELECT * FROM `pkce` WHERE `ClientId` = @clientid";

            object parameters = new
            {
                clientid
            };

            return await _sqlDataAccess.LoadData<Pkce>(sql, parameters);
        }

        public async Task Insert(Pkce pkce)
        {
            string sql = "INSERT INTO `pkce` (ClientId, CodeChallenge) VALUES (@ClientId, @CodeChallenge)";

            await _sqlDataAccess.SaveData(sql, pkce);
        }
    }
}
