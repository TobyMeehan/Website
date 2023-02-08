using Slapper;
using SqlKata;
using SqlKata.Execution;

namespace TobyMeehan.Com.Data.SqlKata;

public abstract class BaseRepository<TData>
{
    protected QueryFactory Db { get; }

    protected BaseRepository(QueryFactory db)
    {
        Db = db;
    }

    protected virtual Query Query()
    {
        return new Query();
    }

    private Query Query(Func<Query, Query>? queryFunc, int page, int perPage)
    {
        var query = Query();

        if (queryFunc is not null)
        {
            query = queryFunc(query);
        }
        
        var clause = query.Clauses.OfType<FromClause>().FirstOrDefault();

        return clause is null 
            ? query.Limit(200) 
            : query.From(new Query(clause.Table), clause.Alias).ForPage(page, perPage);
    }

    protected async Task<List<TData>> QueryAsync(Func<Query, Query>? queryFunc = null, int page = 1, int perPage = 200, CancellationToken cancellationToken = default)
    {
        var query = Query(queryFunc, page, perPage);

        var result = await Db.GetAsync(query, cancellationToken: cancellationToken);
        var items = AutoMapper.MapDynamic<TData>(result);

        return items.ToList();
    }

    protected async Task<TData> QuerySingleAsync(Func<Query, Query>? queryFunc = null, CancellationToken cancellationToken = default)
    {
        var query = Query(queryFunc, 1, 1);

        var result = await Db.FirstOrDefaultAsync(query, cancellationToken: cancellationToken);

        return AutoMapper.MapDynamic<TData>(result);
    }
}