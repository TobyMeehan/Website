using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories;
using TobyMeehan.Com.Domain.Downloads;
using TobyMeehan.Com.Domain.Downloads.Services;

namespace TobyMeehan.Com.Data.Services;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepository;

    public CommentService(ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }
    
    public async Task<Comment> CreateAsync(ICommentService.CreateComment create, CancellationToken cancellationToken = default)
    {
        var comment = new CommentDto
        {
            DownloadId = create.DownloadId,
            UserId = create.UserId,
            Content = create.Content,
            CreatedAt = DateTime.UtcNow
        };
        
        comment = await _commentRepository.CreateAsync(comment, cancellationToken);

        return new Comment
        {
            Id = comment.Id,
            DownloadId = comment.DownloadId,
            UserId = comment.UserId,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt,
            EditedAt = null
        };
    }

    public async Task<IReadOnlyList<Comment>> GetByDownloadAsync(Guid downloadId, CancellationToken cancellationToken = default)
    {
        var comments = await _commentRepository.GetByDownloadAsync(downloadId, cancellationToken);

        return comments.Select(comment => new Comment
        {
            Id = comment.Id,
            DownloadId = comment.DownloadId,
            UserId = comment.UserId,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt,
            EditedAt = comment.EditedAt
        }).ToList();
    }

    public async Task<Comment?> UpdateAsync(Guid id, ICommentService.UpdateComment update, CancellationToken cancellationToken = default)
    {
        var comment = await _commentRepository.GetByIdAsync(id, cancellationToken);

        if (comment is null)
        {
            return null;
        }
        
        comment.Content = update.Content;
        comment.EditedAt = DateTime.UtcNow;
        
        await _commentRepository.UpdateAsync(comment, cancellationToken);

        return new Comment
        {
            Id = comment.Id,
            DownloadId = comment.DownloadId,
            UserId = comment.UserId,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt,
            EditedAt = comment.EditedAt
        };
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _commentRepository.DeleteAsync(id, cancellationToken);
    }
}