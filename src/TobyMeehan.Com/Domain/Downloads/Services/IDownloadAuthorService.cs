namespace TobyMeehan.Com.Domain.Downloads.Services;

public interface IDownloadAuthorService
{
    Task<DownloadAuthor> AddAsync(Guid downloadId, Guid userId, CancellationToken cancellationToken = default);
    
    Task<IReadOnlyList<DownloadAuthor>> GetByDownloadAsync(Guid downloadId, CancellationToken cancellationToken = default);
    
    Task<bool> IsAuthorAsync(Guid downloadId, Guid userId, CancellationToken cancellationToken = default);
    Task<bool> IsOwnerAsync(Guid downloadId, Guid userId, CancellationToken cancellationToken = default);
    
    Task RemoveAsync(Guid downloadId, Guid userId, CancellationToken cancellationToken = default);
}