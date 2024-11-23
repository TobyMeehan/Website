using TobyMeehan.Com.Data.Models;

namespace TobyMeehan.Com.Data.Repositories;

public interface IDownloadRepository
{
    Task<DownloadDto> CreateAsync(DownloadDto download, CancellationToken cancellationToken);
    Task<IReadOnlyList<DownloadDto>> GetPublicAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<DownloadDto>> GetByUserAsync(Guid userId, CancellationToken cancellationToken);
    Task<DownloadDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<DownloadDto?> GetByUrlAsync(string url, CancellationToken cancellationToken);
    Task<DownloadDto> UpdateAsync(DownloadDto download, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}