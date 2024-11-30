using Microsoft.EntityFrameworkCore;
using TobyMeehan.Com.Data.DataAccess;
using TobyMeehan.Com.Data.Models;

namespace TobyMeehan.Com.Data.Repositories.EntityFramework;

public class CommentRepository : ICommentRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly DbSet<CommentDto> _comments;

    public CommentRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        _comments = dbContext.Set<CommentDto>();
    }
    
    public async Task<CommentDto> CreateAsync(CommentDto comment, CancellationToken cancellationToken)
    {
        _comments.Add(comment);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return comment;
    }

    public async Task<IReadOnlyList<CommentDto>> GetByDownloadAsync(Guid downloadId, CancellationToken cancellationToken)
    {
        return await _comments
            .Where(x => !x.Download!.DeletedAt.HasValue)
            .Where(x => x.DownloadId == downloadId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<CommentDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _comments
            .Where(x => !x.Download!.DeletedAt.HasValue)
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task UpdateAsync(CommentDto comment, CancellationToken cancellationToken)
    {
        _comments.Update(comment);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        await _comments
            .Where(x => !x.Download!.DeletedAt.HasValue)
            .Where(x => x.Id == id)
            .ExecuteDeleteAsync(cancellationToken);
    }
}