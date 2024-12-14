using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using TobyMeehan.Com.Domain.Downloads.Services;

namespace TobyMeehan.Com.Features.Comments.GetByDownload;

public class Endpoint : Endpoint<Request, List<CommentResponse>, CommentMapper>
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
        Get("/downloads/{DownloadId}/comments");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var download = await _downloadService.GetByPublicIdAsync(req.DownloadId, ct);

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
        
        var comments = await _commentService.GetByDownloadAsync(download.Id, ct);

        Response = comments.Select(Map.FromEntity).ToList();
    }
}