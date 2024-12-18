using TobyMeehan.Com.Data.Models;

namespace TobyMeehan.Com.Data.Repositories;

public interface ICommentRepository
{
    Task<CommentDto> CreateAsync(CommentDto comment, CancellationToken cancellationToken);
    Task<IReadOnlyList<CommentDto>> GetByDownloadAsync(Guid downloadId, CancellationToken cancellationToken);
    Task<IReadOnlyList<CommentDto>> GetRepliesAsync(Guid commentId, CancellationToken cancellationToken);
    Task<CommentDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task UpdateAsync(CommentDto comment, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}