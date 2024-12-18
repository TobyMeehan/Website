using Microsoft.EntityFrameworkCore;
using TobyMeehan.Com.Data.DataAccess;
using TobyMeehan.Com.Data.Models;

namespace TobyMeehan.Com.Data.Repositories.EntityFramework;

public class DownloadAuthorRepository : IDownloadAuthorRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly DbSet<DownloadAuthorDto> _downloadAuthors;

    public DownloadAuthorRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        _downloadAuthors = dbContext.Set<DownloadAuthorDto>();
    }
    
    public async Task<DownloadAuthorDto> AddAsync(DownloadAuthorDto author, CancellationToken cancellationToken)
    {
        _downloadAuthors.Add(author);
        
        await _dbContext.SaveChangesAsync(cancellationToken);

        return author;
    }

    public async Task<DownloadAuthorDto?> GetAsync(Guid downloadId, Guid userId, CancellationToken cancellationToken)
    {
        return await _downloadAuthors
            .Where(x => !x.Download!.DeletedAt.HasValue)
            .Where(x => x.DownloadId == downloadId && x.UserId == userId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DownloadAuthorDto>> GetByDownloadAsync(Guid downloadId, CancellationToken cancellationToken)
    {
        return await _downloadAuthors
            .Where(x => !x.Download!.DeletedAt.HasValue)
            .Where(x => x.DownloadId == downloadId)
            .ToListAsync(cancellationToken);
    }

    public async Task RemoveAsync(Guid downloadId, Guid userId, CancellationToken cancellationToken)
    {
        await _downloadAuthors
            .Where(x => !x.Download!.DeletedAt.HasValue)
            .Where(x => x.DownloadId == downloadId && x.UserId == userId)
            .ExecuteDeleteAsync(cancellationToken);
    }
}