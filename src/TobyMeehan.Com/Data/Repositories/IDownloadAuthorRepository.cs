using TobyMeehan.Com.Data.Models;

namespace TobyMeehan.Com.Data.Repositories;

public interface IDownloadAuthorRepository
{
    Task<DownloadAuthorDto> AddAsync(DownloadAuthorDto author, CancellationToken cancellationToken);
    Task<DownloadAuthorDto?> GetAsync(Guid downloadId, Guid userId, CancellationToken cancellationToken);
    Task<IReadOnlyList<DownloadAuthorDto>> GetByDownloadAsync(Guid downloadId, CancellationToken cancellationToken);
    Task RemoveAsync(Guid downloadId, Guid userId, CancellationToken cancellationToken);
}