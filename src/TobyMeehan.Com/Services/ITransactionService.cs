using OneOf;
using TobyMeehan.Com.Models.Transaction;
using TobyMeehan.Com.Results;

namespace TobyMeehan.Com.Services;

public interface ITransactionService
{
    IAsyncEnumerable<ITransaction> GetByUserAsync(Id<IUser> user, QueryOptions? options = null,
        CancellationToken cancellationToken = default);
    
    Task<OneOf<ITransaction, NotFound>>
        GetByIdAndUserAsync(Id<ITransaction> id, Id<IUser> user, QueryOptions? options = null, CancellationToken cancellationToken = default);
    
    Task<OneOf<ITransaction, InsufficientBalance, NotFound>> CreateAsync(ICreateTransaction transaction, CancellationToken cancellationToken = default);

    Task<OneOf<ITransaction, InsufficientBalance, NotFound>> CreateTransferAsync(ICreateTransfer transfer, CancellationToken cancellationToken = default);
}