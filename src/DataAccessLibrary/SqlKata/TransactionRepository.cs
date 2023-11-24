using SqlKata;
using TobyMeehan.Com.Data.DataAccess;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Data.SqlKata;

public class TransactionRepository : Repository<TransactionDto>, ITransactionRepository
{
    public TransactionRepository(ISqlDataAccess db) : base(db, "transactions")
    {
    }

    protected override Query Query()
    {
        return base.Query()
            .OrderByDesc("SentAt");
    }

    public IAsyncEnumerable<TransactionDto> SelectByUserAsync(string userId, LimitStrategy? limit, CancellationToken ct)
    {
        return Db.QueryAsync<TransactionDto>(Query(limit)
                .Where(Column("UserId"), userId)
                .OrWhere(Column("RecipientId"), userId),
            cancellationToken: ct);
    }
}