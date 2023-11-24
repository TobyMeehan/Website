using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Data.Repositories;

public interface ITransactionRepository
{
    IAsyncEnumerable<TransactionDto> SelectByUserAsync(string userId, LimitStrategy? limit,
        CancellationToken cancellationToken);

    Task<TransactionDto?> SelectByIdAsync(string id, CancellationToken cancellationToken);

    Task<int> InsertAsync(TransactionDto data, CancellationToken cancellationToken);
}