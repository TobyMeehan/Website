using DataAccessLibrary.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLibrary.Storage
{
    public interface ITransactionTable
    {
        Task DeleteById(string id);
        Task DeleteByUser(string userid);
        Task Insert(Transaction transaction);
        Task<List<Transaction>> SelectById(string id);
        Task<List<Transaction>> SelectByUser(string userid);
    }
}