using SqlKata;
using SqlKata.Execution;
using TobyMeehan.Com.Data.Repositories.Models;

namespace TobyMeehan.Com.Data.SqlKata;

public class Repository<T> : BaseRepository<T> where T : IData
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

    public async Task<List<T>> SelectAllAsync(CancellationToken cancellationToken = default)
    {
        return await QueryAsync(cancellationToken: cancellationToken);
    }

    public async Task<T> SelectByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await QuerySingleAsync(query => query.Where($"{Table}.Id", id), cancellationToken: cancellationToken);
    }

    public async Task InsertAsync(T data, CancellationToken cancellationToken = default)
    {
        await Db.Query(Table).InsertAsync(data, cancellationToken: cancellationToken);
    }

    public async Task UpdateAsync(T data, CancellationToken cancellationToken = default)
    {
        await Db.Query(Table).Where("Id", data.Id).UpdateAsync(data, cancellationToken: cancellationToken);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        await Db.Query(Table).Where("Id", id).DeleteAsync(cancellationToken: cancellationToken);
    }
}