using TobyMeehan.Com.Data.Models;

namespace TobyMeehan.Com.Data.Repositories;

public interface IDownloadFileRepository
{
    Task<DownloadFileDto> CreateAsync(DownloadFileDto file, CancellationToken cancellationToken);
    Task<DownloadFileDto?> GetByFilenameAsync(Guid downloadId, string filename, CancellationToken cancellationToken);
    Task<IReadOnlyList<DownloadFileDto>> GetByDownloadAsync(Guid downloadId, CancellationToken cancellationToken);
    Task<DownloadFileDto?> GetByIdAsync(Guid downloadId, Guid id, CancellationToken cancellationToken);
    Task UpdateAsync(DownloadFileDto file, CancellationToken cancellationToken);
    Task DeleteAsync(Guid downloadId, Guid fileId, CancellationToken cancellationToken);
}