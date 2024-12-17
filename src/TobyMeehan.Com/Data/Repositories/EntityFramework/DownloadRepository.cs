using Microsoft.EntityFrameworkCore;
using TobyMeehan.Com.Data.DataAccess;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Domain.Downloads;

namespace TobyMeehan.Com.Data.Repositories.EntityFramework;

public class DownloadRepository : IDownloadRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly DbSet<DownloadDto> _downloads;

    public DownloadRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        _downloads = dbContext.Set<DownloadDto>();
    }

    public async Task<DownloadDto> CreateAsync(DownloadDto download, CancellationToken cancellationToken)
    {
        _downloads.Add(download);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return download;
    }

    public async Task<IReadOnlyList<DownloadDto>> GetPublicAsync(CancellationToken cancellationToken)
    {
        return await _downloads
            .Where(x => !x.DeletedAt.HasValue)
            .Where(x => x.Visibility == Visibility.Public)
            .OrderByDescending(x => x.Files.SelectMany(f => f.Downloads).Count())
            .ThenByDescending(x => x.UpdatedAt ?? x.CreatedAt ?? DateTime.UnixEpoch)
            .ThenBy(x => x.Title)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DownloadDto>> GetByUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _downloads
            .Where(x => !x.DeletedAt.HasValue)
            .Where(x => x.OwnerId == userId || x.Authors.Any(a => a.UserId == userId))
            .OrderByDescending(x => x.Files.SelectMany(f => f.Downloads).Count())
            .ThenByDescending(x => x.UpdatedAt ?? x.CreatedAt ?? DateTime.UnixEpoch)
            .ThenBy(x => x.Title)
            .ToListAsync(cancellationToken);
    }

    public async Task<DownloadDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _downloads
            .Where(x => !x.DeletedAt.HasValue)
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<DownloadDto?> GetByPublicIdAsync(string publicId, CancellationToken cancellationToken)
    {
        return await _downloads
            .Where(x => !x.DeletedAt.HasValue)
            .Where(x => x.PublicId == publicId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<DownloadDto> UpdateAsync(DownloadDto download, CancellationToken cancellationToken)
    {
        _downloads.Update(download);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return download;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        await _downloads
            .Where(x => !x.DeletedAt.HasValue)
            .Where(x => x.Id == id)
            .ExecuteUpdateAsync(x => 
                x.SetProperty(d => d.DeletedAt, DateTime.UtcNow), cancellationToken);
    }
}