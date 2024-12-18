using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using TobyMeehan.Com.Domain.Downloads.Services;

namespace TobyMeehan.Com.Features.Comments.Delete;

public class Endpoint : Endpoint<Request>
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
        Delete("/comments/{CommentId}");
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
            await _authorizationService.AuthorizeAsync(User, comment, Security.Policies.DeleteComment);

        if (!authorizationResult.Succeeded)
        {
            await SendForbiddenAsync(ct);
            return;
        }
        
        await _commentService.DeleteAsync(comment.Id, ct);

        await SendNoContentAsync(ct);
    }
}