using FastEndpoints;
using TobyMeehan.Com.Api.Security;
using TobyMeehan.Com.Builders.Download;
using TobyMeehan.Com.Services;
using IAuthorizationService = Microsoft.AspNetCore.Authorization.IAuthorizationService;

namespace TobyMeehan.Com.Api.Features.Downloads.Create;

public class Endpoint : Endpoint<Request, DownloadResponse>
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
        Post("/downloads");
        Policies(PolicyNames.Download.Scope.Create);
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
            user => AuthorizeAsync(user, req, ct),
            notFound => SendUnauthorizedAsync(ct));
    }

    private async Task AuthorizeAsync(IUser user, Request req, CancellationToken ct)
    {
        var authorizationResult = await _authorizationService.AuthorizeAsync(User, new Resource<IDownload>(user),
            PolicyNames.Download.Operation.Create);

        if (authorizationResult.Succeeded)
        {
            await CreateAsync(user, req, ct);
            return;
        }

        await SendNotFoundAsync(ct);
    }

    private async Task CreateAsync(IUser user, Request req, CancellationToken ct)
    {
        var download = await _service.CreateAsync(new CreateDownloadBuilder()
            .WithUser(user.Id)
            .WithTitle(req.Title)
            .WithSummary(req.Summary)
            .WithDescription(req.Description | null as string)
            .WithVisibility(req.Visibility | VisibilityNames.Public), ct);

        await SendAsync(new DownloadResponse
        {
            Id = download.Id.Value,
            Title = download.Title,
            Summary = download.Summary,
            Description = download.Description,
            Verification = download.Verification,
            Visibility = download.Visibility,
            UpdatedAt = download.UpdatedAt
        }, cancellation: ct);
    }
}