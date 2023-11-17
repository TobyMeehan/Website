using SqlKata;
using SqlKata.Execution;
using TobyMeehan.Com.Data.DataAccess;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Data.SqlKata;

public class Repository<T>
{
    protected ISqlDataAccess Db { get; }
    protected string Table { get; }

    public Repository(ISqlDataAccess db, string? table = null)
    {
        Db = db;
        Table = table ?? typeof(T).Name;
    }

    protected virtual Query Query()
    {
        return new Query(Table);
    }

    protected Query Query(LimitStrategy? limit)
    {
        limit ??= new DefaultLimitStrategy();
        
        return Query()
            .Offset(limit.GetOffset())
            .Limit(limit.GetLimit());
    }

    protected string Column(string column)
    {
        return $"{Table}.{column}";
    }

    public virtual IAsyncEnumerable<T> SelectAllAsync(LimitStrategy? limit, CancellationToken cancellationToken)
    {
        return Db.QueryAsync<T>(Query(limit), cancellationToken);
    }
    
    public virtual async Task<T?> SelectByIdAsync(string id, CancellationToken cancellationToken)
    {
        return await Db.SingleAsync<T>(Query()
                .Where(Column("Id"), id), 
            cancellationToken);
    }

    public virtual async Task<long> CountAsync(CancellationToken cancellationToken)
    {
        return await Db.SingleAsync<long>(Query()
                .AsCount(), 
            cancellationToken: cancellationToken);
    }

    public virtual async Task<int> InsertAsync(T data, CancellationToken cancellationToken)
    {
        return await Db.ExecuteAsync(Query()
                .AsInsert(data), 
            cancellationToken);
    }

    public virtual async Task<int> UpdateAsync(string id, T data, CancellationToken cancellationToken)
    {
        return await Db.ExecuteAsync(Query()
                .AsUpdate(data)
                .Where(Column("Id"), id), 
            cancellationToken);
    }

    public virtual async Task<int> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        return await Db.ExecuteAsync(Query()
                .AsDelete()
                .Where(Column("Id"), id), 
            cancellationToken);
    }
}