using TobyMeehan.Com.Data.Repositories.Models;

namespace TobyMeehan.Com.Data.Repositories;

/// <summary>
/// Database repository for transactions.
/// </summary>
public interface ITransactionRepository
{
    // SELECT

    /// <summary>
    /// Selects the transactions of the specified user.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<List<TransactionData>> SelectByUserAsync(string userId);

    /// <summary>
    /// Selects the transaction with the specified ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<TransactionData> SelectByIdAsync(string id);
    
    // INSERT

    /// <summary>
    /// Inserts the specified transaction data.
    /// </summary>
    /// <param name="transaction"></param>
    /// <returns></returns>
    Task InsertAsync(TransactionData transaction);
}