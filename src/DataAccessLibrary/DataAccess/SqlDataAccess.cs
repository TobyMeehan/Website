using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;
using Dapper;
using Slapper;
using SqlKata;
using SqlKata.Compilers;

namespace TobyMeehan.Com.Data.DataAccess;

public class SqlDataAccess : ISqlDataAccess
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly Compiler _compiler;

    public SqlDataAccess(IDbConnectionFactory connectionFactory, Compiler compiler)
    {
        _connectionFactory = connectionFactory;
        _compiler = compiler;
    }

    public IAsyncEnumerable<T> QueryAsync<T>(Query query, CancellationToken cancellationToken = default)
    {
        var sql = _compiler.Compile(query);

        return QueryAsync<T>(sql.Sql, sql.NamedBindings, cancellationToken);
    }
    
    public async IAsyncEnumerable<T> QueryAsync<T>(string sql, object parameters,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);

        var result = QueryRowsAsync<T>(connection, sql, parameters, cancellationToken);

        await foreach (var item in result)
            yield return item;
    }

    private static async IAsyncEnumerable<T> QueryRowsAsync<T>(DbConnection connection, string sql, object parameters, 
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await using var reader = await connection.ExecuteReaderAsync(sql, parameters);

        var rowParser = reader.GetRowParser<dynamic>();

        while (await reader.ReadAsync(cancellationToken))
        {
            dynamic? item = rowParser.Invoke(reader);
            yield return AutoMapper.MapDynamic<T>(item);
        }
    }

    private static async IAsyncEnumerable<T> QueryListAsync<T>(IDbConnection connection, string sql, object parameters, 
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var result = await connection.QueryAsync(sql, parameters);
        var items = AutoMapper.MapDynamic<T>(result);

        foreach (var item in items)
        {
            yield return item;
        }
    }
    
    public async Task<T?> SingleAsync<T>(Query query, CancellationToken cancellationToken = default)
    {
        var sql = _compiler.Compile(query);

        return await SingleAsync<T>(sql.Sql, sql.NamedBindings, cancellationToken);
    }
    
    public async Task<T?> SingleAsync<T>(string sql, object parameters, CancellationToken cancellationToken = default)
    {
        await foreach (var item in QueryAsync<T>(sql, parameters, cancellationToken))
        {
            return item;
        }

        return default;
    }
    
    public async Task<int> ExecuteAsync(Query query, CancellationToken cancellationToken = default)
    {
        var sql = _compiler.Compile(query);

        return await ExecuteAsync(sql.Sql, sql.NamedBindings, cancellationToken);
    }

    public async Task<int> ExecuteAsync(string sql, object parameters, CancellationToken cancellationToken = default)
    {
        int result;

        try
        {
            await using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);

            result = await connection.ExecuteAsync(sql, parameters);
        }
        catch (NotSupportedException)
        {
            using var connection = _connectionFactory.CreateConnection();

            result = await connection.ExecuteAsync(sql, parameters);
        }

        return result;
    }
}