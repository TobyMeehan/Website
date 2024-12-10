using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using TobyMeehan.Com.Domain.Downloads.Services;

namespace TobyMeehan.Com.Features.Comments.GetReplies;

public class Endpoint : Endpoint<Request, List<CommentResponse>, CommentMapper>
{
    private readonly ICommentService _commentService;
    private readonly IAuthorizationService _authorizationService;

    public Endpoint(
        ICommentService commentService,
        IAuthorizationService authorizationService)
    {
        _commentService = commentService;
        _authorizationService = authorizationService;
    }

    public override void Configure()
    {
        Get("/comments/{CommentId}/replies");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var comment = await _commentService.GetByIdAsync(req.CommentId, ct);

        if (comment is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var authorizationResult =
            await _authorizationService.AuthorizeAsync(User, comment, Security.Policies.ViewComment);

        if (!authorizationResult.Succeeded)
        {
            await SendForbiddenAsync(ct);
            return;
        }

        var replies = await _commentService.GetRepliesAsync(comment.Id, ct);

        Response = replies.Select(Map.FromEntity).ToList();
    }
}