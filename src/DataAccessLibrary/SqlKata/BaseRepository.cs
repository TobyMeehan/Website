using Slapper;
using SqlKata;
using SqlKata.Execution;

namespace TobyMeehan.Com.Data.SqlKata;

public abstract class BaseRepository<TData>
{
    private readonly QueryFactory _db;

    protected BaseRepository(QueryFactory db)
    {
        _db = db;
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

    protected async Task<List<TData>> SelectAsync(Func<Query, Query>? queryFunc = null, int page = 1, int perPage = 200)
    {
        var query = Query(queryFunc, page, perPage);

        var result = await _db.GetAsync(query);
        var items = AutoMapper.MapDynamic<TData>(result);

        return items.ToList();
    }

    protected async Task<TData> SelectSingleAsync(Func<Query, Query>? queryFunc = null)
    {
        var query = Query(queryFunc, 1, 1);

        dynamic result = await _db.FirstOrDefaultAsync(query);

        return AutoMapper.MapDynamic<TData>(result);
    }
}