using System.Data;
using System.Data.Common;
using Npgsql;

namespace TobyMeehan.Com.Data.DataAccess;

public class PostgresConnectionFactory : IDbConnectionFactory
{
    private readonly NpgsqlDataSource _dataSource;

    public PostgresConnectionFactory(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }
    
    public IDbConnection CreateConnection()
    {
        return _dataSource.OpenConnection();
    }

    public async Task<DbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default)
    {
        return await _dataSource.OpenConnectionAsync(cancellationToken);
    }
}