using TobyMeehan.Com.Data.Domain.Downloads.Models;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Data.Domain.Downloads.Repositories;

public interface IDownloadAuthorRepository
{
    IAsyncEnumerable<AuthorDto> SelectByDownloadAsync(string downloadId, LimitStrategy? limit, CancellationToken cancellationToken);

    Task<AuthorDto?> SelectAsync(string downloadId, string userId, CancellationToken cancellationToken);

    Task<int> InsertAsync(AuthorDto data, CancellationToken cancellationToken);
    
    Task<int> DeleteAsync(string downloadId, string userId, CancellationToken cancellationToken);
}