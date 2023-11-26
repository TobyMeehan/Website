using System.Transactions;
using OneOf;
using TobyMeehan.Com.Data.Caching;
using TobyMeehan.Com.Data.Domain.Transactions.Models;
using TobyMeehan.Com.Data.Domain.Transactions.Repositories;
using TobyMeehan.Com.Data.Domain.Users.Repositories;
using TobyMeehan.Com.Models.Transaction;
using TobyMeehan.Com.Results;
using TobyMeehan.Com.Services;
using Transaction = TobyMeehan.Com.Data.Domain.Transactions.Models.Transaction;

namespace TobyMeehan.Com.Data.Domain.Transactions;

public class TransactionService : BaseService<Transaction, ITransaction, TransactionDto>, ITransactionService
{
    private readonly ITransactionRepository _db;
    private readonly IUserRepository _userRepo;
    private readonly IIdService _id;

    public TransactionService(
        ITransactionRepository db,
        IUserRepository userRepo,
        IIdService id,
        ICacheService<TransactionDto, Id<ITransaction>> cache) : base(cache)
    {
        _db = db;
        _userRepo = userRepo;
        _id = id;
    }

    protected override async Task<Transaction> MapAsync(TransactionDto data)
    {
        return new Transaction
        {
            Id = new Id<ITransaction>(data.Id),
            RecipientId = new Id<IUser>(data.RecipientId ?? data.UserId),
            SenderId = data.RecipientId is not null ? new Id<IUser>(data.UserId) : null,
            ApplicationId = new Id<IApplication>(data.ApplicationId),
            Description = data.Description,
            Amount = data.Amount,
            SentAt = data.SentAt
        };
    }
    
    public IAsyncEnumerable<ITransaction> GetByUserAsync(Id<IUser> user, QueryOptions? options = null, CancellationToken cancellationToken = default)
    {
        var data = _db.SelectByUserAsync(user.Value, options?.LimitStrategy, cancellationToken);

        return GetAsync(data);
    }

    public async Task<OneOf<ITransaction, NotFound>> GetByIdAndUserAsync(Id<ITransaction> id, Id<IUser> user, QueryOptions? options = null, CancellationToken cancellationToken = default)
    {
        var data = Cache.Get(id) ?? await _db.SelectByIdAsync(id.Value, cancellationToken);

        if (data?.Id != user.Value)
        {
            return new NotFound();
        }

        return await GetAsync(data);
    }

    public async Task<OneOf<ITransaction, InsufficientBalance, NotFound>> CreateAsync(ICreateTransaction transaction, CancellationToken cancellationToken = default)
    {
        var user = await _userRepo.SelectByIdAsync(transaction.User.Value, cancellationToken);

        if (user is null)
        {
            return new NotFound();
        }

        user.Balance += transaction.Amount;

        if (user.Balance < 0)
        {
            return new InsufficientBalance();
        }
        
        var id = await _id.GenerateAsync<ITransaction>();

        var data = new TransactionDto
        {
            Id = id.Value,
            UserId = transaction.User.Value,
            ApplicationId = transaction.Application.Value,
            Description = transaction.Description,
            Amount = transaction.Amount,
            SentAt = DateTime.UtcNow
        };

        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            await _db.InsertAsync(data, cancellationToken);
            await _userRepo.UpdateAsync(user.Id, user, cancellationToken);
            
            scope.Complete();
        }

        return await GetAsync(data);
    }

    public async Task<OneOf<ITransaction, InsufficientBalance, NotFound>> CreateTransferAsync(ICreateTransfer transfer, CancellationToken cancellationToken = default)
    {
        var recipient = await _userRepo.SelectByIdAsync(transfer.Recipient.Value, cancellationToken);
        var sender = await _userRepo.SelectByIdAsync(transfer.Sender.Value, cancellationToken);

        if (recipient is null || sender is null)
        {
            return new NotFound();
        }

        recipient.Balance += transfer.Amount;
        sender.Balance -= transfer.Amount;

        if (sender.Balance < 0)
        {
            return new InsufficientBalance();
        }

        var id = await _id.GenerateAsync<ITransaction>();

        var data = new TransactionDto
        {
            Id = id.Value,
            RecipientId = recipient.Id,
            UserId = sender.Id,
            ApplicationId = transfer.Application.Value,
            Description = transfer.Description,
            Amount = transfer.Amount,
            SentAt = DateTime.UtcNow
        };

        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            await _db.InsertAsync(data, cancellationToken);
            await _userRepo.UpdateAsync(recipient.Id, recipient, cancellationToken);
            await _userRepo.UpdateAsync(sender.Id, sender, cancellationToken);
            
            scope.Complete();
        }
        
        return await GetAsync(data);
    }
}