using System.Security.Claims;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using TobyMeehan.Com.Domain.Downloads.Services;

namespace TobyMeehan.Com.Features.Comments.PostReply;

public class Endpoint : Endpoint<Request, CommentResponse, CommentMapper>
{
    private readonly ICommentService _commentService;
    private readonly IAuthorizationService _authorizationService;

    public Endpoint(ICommentService commentService, IAuthorizationService authorizationService)
    {
        _commentService = commentService;
        _authorizationService = authorizationService;
    }

    public override void Configure()
    {
        Post("/comments/{CommentId}/replies");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            throw new InvalidOperationException();
        }

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

        var reply = await _commentService.CreateAsync(comment, new ICommentService.CreateComment(
                UserId: userId,
                Content: req.Content),
            ct);
        
        await SendCreatedAtAsync<GetById.Endpoint>(new { CommentId = comment.Id },
            Map.FromEntity(comment), cancellation: ct);
    }
}