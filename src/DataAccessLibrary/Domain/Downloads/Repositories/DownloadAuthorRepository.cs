using TobyMeehan.Com.Data.DataAccess;
using TobyMeehan.Com.Data.Domain.Downloads.Models;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Data.Domain.Downloads.Repositories;

public class DownloadAuthorRepository : Repository<AuthorDto>, IDownloadAuthorRepository
{
    public DownloadAuthorRepository(ISqlDataAccess db) : base(db, "downloadauthors")
    {
    }

    public IAsyncEnumerable<AuthorDto> SelectByDownloadAsync(string downloadId, LimitStrategy? limit, CancellationToken ct)
    {
        return Db.QueryAsync<AuthorDto>(Query(limit)
                .Where(Column("DownloadId"), downloadId),
            cancellationToken: ct);
    }

    public async Task<AuthorDto?> SelectAsync(string downloadId, string userId, CancellationToken ct)
    {
        return await Db.SingleAsync<AuthorDto>(Query()
                .Where(Column("DownloadId"), downloadId)
                .Where(Column("UserId"), userId),
            cancellationToken: ct);
    }

    public async Task<int> DeleteAsync(string downloadId, string userId, CancellationToken ct)
    {
        return await Db.ExecuteAsync(Query()
                .AsDelete()
                .Where(Column("DownloadId"), downloadId)
                .Where(Column("UserId"), userId),
            cancellationToken: ct);
    }
}