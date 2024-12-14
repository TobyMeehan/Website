using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories;
using TobyMeehan.Com.Domain.Downloads;
using TobyMeehan.Com.Domain.Downloads.Services;

namespace TobyMeehan.Com.Data.Services;

public class DownloadService : IDownloadService
{
    private readonly IDownloadRepository _downloadRepository;

    public DownloadService(IDownloadRepository downloadRepository)
    {
        _downloadRepository = downloadRepository;
    }

    public async Task<Download> CreateAsync(IDownloadService.CreateDownload create,
        CancellationToken cancellationToken = default)
    {
        var publicId = new string(Random.Shared.GetItems(
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".AsSpan(),
            length: 11));

        var download = new DownloadDto
        {
            OwnerId = create.UserId,
            PublicId = publicId,
            Title = create.Title,
            Summary = create.Summary,
            Description = create.Description,
            Visibility = create.Visibility,
            CreatedAt = DateTime.UtcNow,
        };

        download = await _downloadRepository.CreateAsync(download, cancellationToken);

        return new Download
        {
            Id = download.Id,
            PublicId = download.PublicId,
            Title = download.Title,
            Summary = download.Summary,
            Description = download.Description,
            Visibility = download.Visibility,
            Verification = download.Verification,
            Version = null,
            CreatedAt = download.CreatedAt,
            UpdatedAt = null
        };
    }

    public async Task<IReadOnlyList<Download>> GetPublicAsync(CancellationToken cancellationToken = default)
    {
        var downloads = await _downloadRepository.GetPublicAsync(cancellationToken);

        return downloads.Select(download => new Download
        {
            Id = download.Id,
            PublicId = download.PublicId,
            Title = download.Title,
            Summary = download.Summary,
            Description = download.Description,
            Visibility = download.Visibility,
            Verification = download.Verification,
            Version = Version.TryParse(download.Version, out var version) ? version : null,
            CreatedAt = download.CreatedAt,
            UpdatedAt = download.UpdatedAt
        }).ToList();
    }

    public async Task<IReadOnlyList<Download>> GetByUserAsync(Guid userId,
        CancellationToken cancellationToken = default)
    {
        var downloads = await _downloadRepository.GetByUserAsync(userId, cancellationToken);

        return downloads.Select(download => new Download
        {
            Id = download.Id,
            PublicId = download.PublicId,
            Title = download.Title,
            Summary = download.Summary,
            Description = download.Description,
            Visibility = download.Visibility,
            Verification = download.Verification,
            Version = Version.TryParse(download.Version, out var version) ? version : null,
            CreatedAt = download.CreatedAt,
            UpdatedAt = download.UpdatedAt
        }).ToList();
    }

    public async Task<Download?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var download = await _downloadRepository.GetByIdAsync(id, cancellationToken);

        if (download is null)
        {
            return null;
        }

        return new Download
        {
            Id = download.Id,
            PublicId = download.PublicId,
            Title = download.Title,
            Summary = download.Summary,
            Description = download.Description,
            Visibility = download.Visibility,
            Verification = download.Verification,
            Version = Version.TryParse(download.Version, out var version) ? version : null,
            CreatedAt = download.CreatedAt,
            UpdatedAt = download.UpdatedAt
        };
    }

    public async Task<Download?> GetByPublicIdAsync(string publicId, CancellationToken cancellationToken = default)
    {
        var download = await _downloadRepository.GetByPublicIdAsync(publicId, cancellationToken);

        if (download is null)
        {
            return null;
        }

        return new Download
        {
            Id = download.Id,
            PublicId = download.PublicId,
            Title = download.Title,
            Summary = download.Summary,
            Description = download.Description,
            Visibility = download.Visibility,
            Verification = download.Verification,
            Version = Version.TryParse(download.Version, out var version) ? version : null,
            CreatedAt = download.CreatedAt,
            UpdatedAt = download.UpdatedAt
        };
    }

    public async Task<Download?> UpdateAsync(Guid id, IDownloadService.UpdateDownload update,
        CancellationToken cancellationToken = default)
    {
        var download = await _downloadRepository.GetByIdAsync(id, cancellationToken);

        if (download is null)
        {
            return null;
        }

        download.Title = update.Title;
        download.Summary = update.Summary;
        download.Description = update.Description;
        download.Visibility = update.Visibility;

        if (Version.TryParse(download.Version, out var currentVersion) && update.Version > currentVersion
            || download.Version is null && update.Version is not null)
        {
            download.UpdatedAt = DateTime.UtcNow;
        }

        download.Version = update.Version?.ToString();

        await _downloadRepository.UpdateAsync(download, cancellationToken);

        return new Download
        {
            Id = download.Id,
            PublicId = download.PublicId,
            Title = download.Title,
            Summary = download.Summary,
            Description = download.Description,
            Visibility = download.Visibility,
            Verification = download.Verification,
            Version = Version.TryParse(download.Version, out var version) ? version : null,
            CreatedAt = download.CreatedAt,
            UpdatedAt = download.UpdatedAt
        };
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _downloadRepository.DeleteAsync(id, cancellationToken);
    }
}