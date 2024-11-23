using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories;
using TobyMeehan.Com.Domain.Downloads;
using TobyMeehan.Com.Domain.Downloads.Services;

namespace TobyMeehan.Com.Data.Services;

public class DownloadAuthorService : IDownloadAuthorService
{
    private readonly IDownloadAuthorRepository _downloadAuthorRepository;
    private readonly IDownloadRepository _downloadRepository;

    public DownloadAuthorService(
        IDownloadAuthorRepository downloadAuthorRepository,
        IDownloadRepository downloadRepository)
    {
        _downloadAuthorRepository = downloadAuthorRepository;
        _downloadRepository = downloadRepository;
    }

    public async Task<DownloadAuthor> AddAsync(Guid downloadId, Guid userId,
        CancellationToken cancellationToken = default)
    {
        var author = await _downloadAuthorRepository.GetAsync(downloadId, userId, cancellationToken);

        if (author is not null)
        {
            return new DownloadAuthor
            {
                DownloadId = author.DownloadId,
                UserId = author.UserId,
                IsOwner = false,
                CreatedAt = author.CreatedAt
            };
        }

        author = new DownloadAuthorDto
        {
            DownloadId = downloadId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        author = await _downloadAuthorRepository.AddAsync(author, cancellationToken);

        return new DownloadAuthor
        {
            DownloadId = author.DownloadId,
            UserId = author.UserId,
            IsOwner = false,
            CreatedAt = author.CreatedAt
        };
    }

    public async Task<IReadOnlyList<DownloadAuthor>> GetByDownloadAsync(Guid downloadId,
        CancellationToken cancellationToken = default)
    {
        var results = new List<DownloadAuthor>();

        var download = await _downloadRepository.GetByIdAsync(downloadId, cancellationToken);

        if (download is null)
        {
            return [];
        }

        results.Add(new DownloadAuthor
        {
            DownloadId = download.Id,
            UserId = download.OwnerId,
            IsOwner = true,
            CreatedAt = download.CreatedAt
        });

        var authors = await _downloadAuthorRepository.GetByDownloadAsync(downloadId, cancellationToken);

        results.AddRange(authors.Select(author => new DownloadAuthor
        {
            DownloadId = download.Id,
            UserId = author.UserId,
            IsOwner = false,
            CreatedAt = author.CreatedAt
        }));

        return results;
    }

    public async Task RemoveAsync(Guid downloadId, Guid userId, CancellationToken cancellationToken = default)
    {
        await _downloadAuthorRepository.RemoveAsync(downloadId, userId, cancellationToken);
    }
}