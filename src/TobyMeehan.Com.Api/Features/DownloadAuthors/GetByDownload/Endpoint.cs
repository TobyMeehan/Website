using FastEndpoints;
using TobyMeehan.Com.Api.Features.Users;
using TobyMeehan.Com.Api.Security;
using TobyMeehan.Com.Services;
using IAuthorizationService = Microsoft.AspNetCore.Authorization.IAuthorizationService;

namespace TobyMeehan.Com.Api.Features.DownloadAuthors.GetByDownload;

public class Endpoint : Endpoint<Request, List<DownloadAuthorResponse>>
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
        Get("/downloads/{DownloadId}/authors");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await _service.GetByIdAsync(new Id<IDownload>(req.DownloadId), cancellationToken: ct);

        await result.Match(
            download => AuthorizeAsync(download, ct),
            notFound => SendNotFoundAsync(ct));
    }

    private async Task AuthorizeAsync(IDownload download, CancellationToken ct)
    {
        var authorizationResult =
            await _authorizationService.AuthorizeAsync(User, download, PolicyNames.Download.Operation.Read);

        if (authorizationResult.Succeeded)
        {
            await GetAsync(download, ct);
            return;
        }

        await SendForbiddenAsync(ct);
    }

    private async Task GetAsync(IDownload download, CancellationToken ct)
    {
        var canViewPermissions =
            await _authorizationService.AuthorizeAsync(User, null, PolicyNames.Download.Scope.Read);

        var authors = new List<DownloadAuthorResponse>();

        foreach (var author in download.Authors)
        {
            var userResult = await _users.GetByIdAsync(author.Id, cancellationToken: ct);

            var response = new DownloadAuthorResponse
            {
                User = userResult.Match<UserResponse?>(
                    user => new UserResponse
                    {
                        Id = user.Id.Value,
                        Username = user.Username,
                        DisplayName = user.DisplayName,
                        Description = user.Description
                    },
                    notFound => null),
                
                CanEdit = canViewPermissions.Succeeded ? author.CanEdit : Optional<bool>.Empty(),
                CanManageAuthors = canViewPermissions.Succeeded ? author.CanManageAuthors : Optional<bool>.Empty(),
                CanManageFiles = canViewPermissions.Succeeded ? author.CanManageFiles : Optional<bool>.Empty(),
                CanDelete = canViewPermissions.Succeeded ? author.CanDelete : Optional<bool>.Empty()
            };
            
            authors.Add(response);
        }

        await SendAsync(authors, cancellation: ct);
    }
}