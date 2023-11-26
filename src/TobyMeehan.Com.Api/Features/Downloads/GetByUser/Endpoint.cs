using FastEndpoints;
using TobyMeehan.Com.Api.CollectionAuthorization;
using TobyMeehan.Com.Api.Security;
using TobyMeehan.Com.Services;
using IAuthorizationService = Microsoft.AspNetCore.Authorization.IAuthorizationService;

namespace TobyMeehan.Com.Api.Features.Downloads.GetByUser;

public class Endpoint : Endpoint<Request, List<DownloadResponse>>
{
    private readonly IDownloadService _service;
    private readonly IUserService _users;
    private readonly IAuthorizationService _authorizationService;

    public Endpoint(IDownloadService service, IUserService users, IAuthorizationService authorizationService)
    {
        _service = service;
        _users = users;
        _authorizationService = authorizationService;
    }

    public override void Configure()
    {
        Get("/users/{UserId}/downloads");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        if (!req.TryGetUserId(out var userId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var result = await _users.GetByIdAsync(userId, cancellationToken: ct);

        await result.Match(
            async user =>
            {
                var downloads = await _service
                    .GetByAuthorAsync(user.Id, cancellationToken: ct)
                    .ToListAsync(ct);

                await AuthorizeAsync(downloads, user, req, ct);
            },
            notFound => SendNotFoundAsync(ct));
    }

    private async Task AuthorizeAsync(IEnumerable<IDownload> downloads, IUser user, Request req, CancellationToken ct)
    {
        var authorizationResult =
            await _authorizationService.AuthorizeAsync(User, downloads, user, PolicyNames.Download.Operation.Read);

        if (authorizationResult.Succeeded)
        {
            await GetAsync(authorizationResult.AuthorizedResources, req, ct);
            return;
        }

        await SendForbiddenAsync(ct);
    }

    private async Task GetAsync(IEnumerable<IDownload> downloads, Request req, CancellationToken ct)
    {
        var canViewNonPublic =
            await _authorizationService.AuthorizeAsync(User, null, PolicyNames.Download.Scope.Read);
        
        await SendAsync(downloads
            .Where(x => x.Visibility != VisibilityNames.Private || (req.IncludePrivate && canViewNonPublic.Succeeded))
            .Where(x => x.Visibility != VisibilityNames.Unlisted || (req.IncludeUnlisted && canViewNonPublic.Succeeded))
            .Where(x => x.Visibility != VisibilityNames.Public || req.IncludePublic)
            .Select(download =>
            new DownloadResponse
            {
                Id = download.Id.Value,
                Title = download.Title,
                Summary = download.Summary,
                Description = download.Description,
                Verification = download.Verification,
                Visibility = download.Visibility,
                UpdatedAt = download.UpdatedAt
            }).ToList(), cancellation: ct);
    }
}