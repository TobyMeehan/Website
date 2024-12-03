using Microsoft.EntityFrameworkCore;
using TobyMeehan.Com.Data.DataAccess;
using TobyMeehan.Com.Data.Models;

namespace TobyMeehan.Com.Data.Repositories.EntityFramework;

public class DownloadFileRepository : IDownloadFileRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly DbSet<DownloadFileDto> _files;

    public DownloadFileRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        _files = _dbContext.Set<DownloadFileDto>();
    }
    
    public async Task<DownloadFileDto> CreateAsync(DownloadFileDto file, CancellationToken cancellationToken)
    {
        _files.Add(file);
        
        await _dbContext.SaveChangesAsync(cancellationToken);

        return file;
    }

    public async Task<DownloadFileDto?> GetByFilenameAsync(Guid downloadId, string filename, CancellationToken cancellationToken)
    {
        return await _files
            .Where(x => x.DownloadId == downloadId)
            .Where(x => x.Filename == filename)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DownloadFileDto>> GetByDownloadAsync(Guid downloadId, CancellationToken cancellationToken)
    {
        return await _files
            .Where(x => x.DownloadId == downloadId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<DownloadFileDto?> GetByIdAsync(Guid downloadId, Guid id, CancellationToken cancellationToken)
    {
        return await _files
            .Where(x => x.DownloadId == downloadId)
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task UpdateAsync(DownloadFileDto file, CancellationToken cancellationToken)
    {
        _files.Update(file);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid downloadId, Guid fileId, CancellationToken cancellationToken)
    {
        await _files
            .Where(x => x.DownloadId == downloadId)
            .Where(x => x.Id == fileId)
            .ExecuteDeleteAsync(cancellationToken);
    }
}