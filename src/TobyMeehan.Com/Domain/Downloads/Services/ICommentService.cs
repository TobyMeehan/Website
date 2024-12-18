namespace TobyMeehan.Com.Domain.Downloads.Services;

public interface ICommentService
{
    public record CreateComment(Guid UserId, string Content);
    Task<Comment> CreateAsync(Download download, CreateComment create, CancellationToken cancellationToken = default);
    Task<Comment> CreateAsync(Comment comment, CreateComment create, CancellationToken cancellationToken = default);
    
    Task<Comment?> GetByIdAsync(Guid commentId, CancellationToken cancellationToken = default);
    
    Task<IReadOnlyList<Comment>> GetByDownloadAsync(Guid downloadId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Comment>> GetRepliesAsync(Guid commentId, CancellationToken cancellationToken = default);

    public record UpdateComment(string Content);
    Task<Comment?> UpdateAsync(Guid id, UpdateComment update, CancellationToken cancellationToken = default);
    
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}