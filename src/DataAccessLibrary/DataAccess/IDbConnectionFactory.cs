using System.Data;
using System.Data.Common;

namespace TobyMeehan.Com.Data.DataAccess;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();

    Task<DbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default);
}