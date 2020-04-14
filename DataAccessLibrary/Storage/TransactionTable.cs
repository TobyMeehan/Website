using DataAccessLibrary.DataAccess;
using DataAccessLibrary.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Storage
{
    public class TransactionTable : ITransactionTable
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public TransactionTable(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<List<Transaction>> SelectById(string id)
        {
            string sql = "SELECT * FROM `transactions` WHERE `Id` = @id";

            object parameters = new
            {
                id
            };

            return await _sqlDataAccess.LoadData<Transaction>(sql, parameters);
        }

        public async Task<List<Transaction>> SelectByUser(string userid)
        {
            string sql = "SELECT * FROM `transactions` WHERE `UserId` = @userid";

            object parameters = new
            {
                userid
            };

            return await _sqlDataAccess.LoadData<Transaction>(sql, parameters);
        }

        public async Task Insert(Transaction transaction)
        {
            string sql = "INSERT INTO `transactions` (Id, UserId, Sender, Description, Amount) VALUES (UUID(), @UserId, @Sender, @Description, @Amount)";

            await _sqlDataAccess.SaveData(sql, transaction);
        }

        public async Task DeleteById(string id)
        {
            string sql = "DELETE FROM `transactions` WHERE `Id` = @id";

            object parameters = new
            {
                id
            };

            await _sqlDataAccess.SaveData(sql, parameters);
        }

        public async Task DeleteByUser(string userid)
        {
            string sql = "DELETE FROM `transactions` WHERE `UserId` = @userid";

            object parameters = new
            {
                userid
            };

            await _sqlDataAccess.SaveData(sql, parameters);
        }
    }
}
