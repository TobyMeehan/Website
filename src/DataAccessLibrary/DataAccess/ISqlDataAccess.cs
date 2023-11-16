using System.Runtime.CompilerServices;
using SqlKata;

namespace TobyMeehan.Com.Data.DataAccess;

public interface ISqlDataAccess
{
    IAsyncEnumerable<T> QueryAsync<T>(Query query, CancellationToken cancellationToken = default);

    IAsyncEnumerable<T> QueryAsync<T>(string sql, object parameters, CancellationToken cancellationToken = default);

    Task<T?> SingleAsync<T>(Query query, CancellationToken cancellationToken = default);
    
    Task<T?> SingleAsync<T>(string sql, object parameters, CancellationToken cancellationToken = default);

    Task<int> ExecuteAsync(Query query, CancellationToken cancellationToken = default);

    Task<int> ExecuteAsync(string sql, object parameters, CancellationToken cancellationToken = default);
}