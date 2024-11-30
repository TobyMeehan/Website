using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using TobyMeehan.Com.Domain.Downloads.Services;

namespace TobyMeehan.Com.Features.Comments.GetById;

public class Endpoint : Endpoint<Request, CommentResponse, CommentMapper>
{
    private readonly IDownloadService _downloadService;
    private readonly ICommentService _commentService;
    private readonly IAuthorizationService _authorizationService;

    public Endpoint(
        IDownloadService downloadService,
        ICommentService commentService,
        IAuthorizationService authorizationService)
    {
        _downloadService = downloadService;
        _commentService = commentService;
        _authorizationService = authorizationService;
    }

    public override void Configure()
    {
        Get("/comments/{CommentId}");
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

        var commentAuthorizationResult =
            await _authorizationService.AuthorizeAsync(User, comment, Security.Policies.ViewComment);

        if (!commentAuthorizationResult.Succeeded)
        {
            await SendForbiddenAsync(ct);
            return;
        }
        
        var download = await _downloadService.GetByIdAsync(comment.DownloadId, ct);

        if (download is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var downloadAuthorizationResult =
            await _authorizationService.AuthorizeAsync(User, download, Security.Policies.ViewDownload);

        if (!downloadAuthorizationResult.Succeeded)
        {
            await SendForbiddenAsync(ct);
            return;
        }

        Response = Map.FromEntity(comment);
    }
}