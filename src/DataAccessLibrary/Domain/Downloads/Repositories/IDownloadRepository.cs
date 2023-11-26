using TobyMeehan.Com.Data.Domain.Downloads.Models;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Data.Domain.Downloads.Repositories;

public interface IDownloadRepository
{
    IAsyncEnumerable<DownloadDto> SelectPublicAsync(LimitStrategy? limit, CancellationToken cancellationToken);

    IAsyncEnumerable<DownloadDto> SelectByAuthorAsync(string userId, LimitStrategy? limit,
        CancellationToken cancellationToken);

    Task<DownloadDto?> SelectByIdAsync(string id, CancellationToken cancellationToken);

    Task<int> InsertAsync(DownloadDto data, CancellationToken cancellationToken);

    Task<int> UpdateAsync(string id, DownloadDto data, CancellationToken cancellationToken);

    Task<int> DeleteAsync(string id, CancellationToken cancellationToken);
}