using FastEndpoints;
using TobyMeehan.Com.Api.Security;
using TobyMeehan.Com.Services;
using IAuthorizationService = Microsoft.AspNetCore.Authorization.IAuthorizationService;

namespace TobyMeehan.Com.Api.Features.Avatars.Get;

public class Endpoint : Endpoint<Request, AvatarResponse>
{
    private readonly IAvatarService _service;
    private readonly IUserService _users;
    private readonly IAuthorizationService _authorizationService;

    public Endpoint(IAvatarService service, IUserService users, IAuthorizationService authorizationService)
    {
        _service = service;
        _users = users;
        _authorizationService = authorizationService;
    }
    
    public override void Configure()
    {
        Get("/users/{UserId}/avatars/{Id}");
        Policies(PolicyNames.User.Scope.Identify);
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
            notFound => SendNotFoundAsync(ct));
    }

    private async Task AuthorizeAsync(IUser user, Request req, CancellationToken ct)
    {
        var authorizationResult =
            await _authorizationService.AuthorizeAsync(User, user, PolicyNames.User.Operation.Identify);

        if (authorizationResult.Succeeded)
        {
            await GetAsync(req, ct);
            return;
        }

        await SendForbiddenAsync(ct);
    }

    private async Task GetAsync(Request req, CancellationToken ct)
    {
        var result = await _service.GetByIdAsync(new Id<IAvatar>(req.Id), cancellationToken: ct);

        await result.Match(
            avatar => SendAsync(new AvatarResponse
            {
                Id = avatar.Id.Value
            }, cancellation: ct),
            notFound => SendNotFoundAsync(ct));
    }
}