namespace TobyMeehan.Com.Domain.Downloads.Services;

public interface IDownloadService
{
    public record CreateDownload(
        Guid UserId,
        string Title,
        string Summary,
        string Description,
        Visibility Visibility);

    Task<Download> CreateAsync(CreateDownload create, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Download>> GetPublicAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Download>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<Download?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Download?> GetByUrlAsync(string url, CancellationToken cancellationToken = default);

    public record UpdateDownload(
        string Title,
        string Summary,
        string Description,
        Visibility Visibility,
        Version? Version);

    Task<Download?> UpdateAsync(Guid id, UpdateDownload update, CancellationToken cancellationToken = default);
    
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}