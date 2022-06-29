using TobyMeehan.Com.Builders;

namespace TobyMeehan.Com.Services;

/// <summary>
/// Service for transaction data.
/// </summary>
public interface ITransactionService
{
    // GET

    /// <summary>
    /// Gets all transactions for the specified user.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    Task<IEntityCollection<ITransaction>> GetByUserAsync(Id<IUser> user);

    /// <summary>
    /// Gets the transaction with the specified ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<ITransaction> GetByIdAsync(Id<ITransaction> id);
    
    // CREATE

    /// <summary>
    /// Creates a new transaction with the specified builder.
    /// </summary>
    /// <param name="transaction"></param>
    /// <returns></returns>
    Task<ITransaction> CreateAsync(CreateTransactionBuilder transaction);
    
    // UPDATE
    
    // DELETE
}