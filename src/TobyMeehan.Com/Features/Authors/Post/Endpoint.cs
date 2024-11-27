using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using TobyMeehan.Com.Domain.Downloads.Services;
using TobyMeehan.Com.Domain.Thavyra.Services;

namespace TobyMeehan.Com.Features.Authors.Post;

public class Endpoint : Endpoint<Request, DownloadAuthorResponse, DownloadAuthorMapper>
{
    private readonly IDownloadService _downloadService;
    private readonly IDownloadAuthorService _downloadAuthorService;
    private readonly IUserService _userService;
    private readonly IAuthorizationService _authorizationService;

    public Endpoint(
        IDownloadService downloadService,
        IDownloadAuthorService downloadAuthorService,
        IUserService userService,
        IAuthorizationService authorizationService)
    {
        _downloadService = downloadService;
        _downloadAuthorService = downloadAuthorService;
        _userService = userService;
        _authorizationService = authorizationService;
    }

    public override void Configure()
    {
        Post("/downloads/{DownloadId}/authors");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var download = await _downloadService.GetByUrlAsync(req.DownloadId, ct);

        if (download is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var authorizationResult =
            await _authorizationService.AuthorizeAsync(User, download, Security.Policies.InviteAuthor);

        if (!authorizationResult.Succeeded)
        {
            await SendForbiddenAsync(ct);
            return;
        }

        var user = await _userService.GetByIdAsync(req.UserId, ct);

        if (user is null)
        {
            ThrowError(x => x.UserId, "User does not exist.");
        }

        var author = await _downloadAuthorService.AddAsync(download.Id, req.UserId, ct);

        await SendCreatedAtAsync<GetById.Endpoint>(new { DownloadId = download.Url, req.UserId },
            Map.FromEntity(author), cancellation: ct);
    }
}