using System.Security.Claims;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using TobyMeehan.Com.Domain.Downloads.Services;

namespace TobyMeehan.Com.Features.Comments.Post;

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
        Post("/downloads/{DownloadId}/comments");
        Policies(Security.Policies.CreateComment);
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            throw new InvalidOperationException();
        }

        var download = await _downloadService.GetByUrlAsync(req.DownloadId, ct);

        if (download is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var authorizationResult =
            await _authorizationService.AuthorizeAsync(User, download, Security.Policies.ViewDownload);

        if (!authorizationResult.Succeeded)
        {
            await SendForbiddenAsync(ct);
            return;
        }

        var comment = await _commentService.CreateAsync(download, new ICommentService.CreateComment(
                userId,
                req.Content),
            ct);

        await SendCreatedAtAsync<GetById.Endpoint>(new { CommentId = comment.Id },
            Map.FromEntity(comment), cancellation: ct);
    }
}