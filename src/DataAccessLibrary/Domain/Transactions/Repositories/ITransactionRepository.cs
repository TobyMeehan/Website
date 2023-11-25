using TobyMeehan.Com.Data.Domain.Transactions.Models;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Data.Domain.Transactions.Repositories;

public interface ITransactionRepository
{
    IAsyncEnumerable<TransactionDto> SelectByUserAsync(string userId, LimitStrategy? limit,
        CancellationToken cancellationToken);

    Task<TransactionDto?> SelectByIdAsync(string id, CancellationToken cancellationToken);

    Task<int> InsertAsync(TransactionDto data, CancellationToken cancellationToken);
}