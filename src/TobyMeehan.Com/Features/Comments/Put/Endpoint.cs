using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using TobyMeehan.Com.Domain.Downloads.Services;

namespace TobyMeehan.Com.Features.Comments.Put;

public class Endpoint : Endpoint<Request, CommentResponse, CommentMapper>
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
        Put("/comments/{Id}");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var comment = await _commentService.GetByIdAsync(req.Id, ct);

        if (comment is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var authorizationResult =
            await _authorizationService.AuthorizeAsync(User, comment, Security.Policies.EditComment);

        if (!authorizationResult.Succeeded)
        {
            await SendForbiddenAsync(ct);
            return;
        }

        comment = await _commentService.UpdateAsync(comment.Id, new ICommentService.UpdateComment(req.Content), ct);

        Response = Map.FromEntity(comment!);
    }
}