using SqlKata;
using SqlKata.Execution;
using TobyMeehan.Com.Data.Repositories.Models;

namespace TobyMeehan.Com.Data.SqlKata;

public class Repository<T> : BaseRepository<T>
{
    protected string Table { get; }

    public Repository(QueryFactory db, string? table = null) : base(db)
    {
        Table = table ?? typeof(T).Name;
    }

    protected override Query Query()
    {
        return base.Query()
            .From(Table);
    }

    public async Task<List<T>> SelectAllAsync(CancellationToken cancellationToken)
    {
        return await QueryAsync(cancellationToken: cancellationToken);
    }

    public async Task<T?> SelectByIdAsync(string id, CancellationToken cancellationToken)
    {
        return await QuerySingleAsync(query => query.Where($"{Table}.Id", id), cancellationToken: cancellationToken);
    }

    public async Task InsertAsync(T data, CancellationToken cancellationToken)
    {
        await Db.Query(Table).InsertAsync(data, cancellationToken: cancellationToken);
    }

    public async Task UpdateAsync(string id, T data, CancellationToken cancellationToken)
    {
        await Db.Query(Table).Where("Id", id).UpdateAsync(data, cancellationToken: cancellationToken);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken)
    {
        await Db.Query(Table).Where("Id", id).DeleteAsync(cancellationToken: cancellationToken);
    }
}