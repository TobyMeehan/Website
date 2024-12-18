using FastEndpoints;
using TobyMeehan.Com.Domain.Downloads;

namespace TobyMeehan.Com.Features.Comments;

public class CommentMapper : ResponseMapper<CommentResponse, Comment>
{
    public override CommentResponse FromEntity(Comment e)
    {
        return new CommentResponse
        {
            Id = e.Id,
            UserId = e.UserId,
            Content = e.Content,
            CreatedAt = e.CreatedAt,
            EditedAt = e.EditedAt
        };
    }
}