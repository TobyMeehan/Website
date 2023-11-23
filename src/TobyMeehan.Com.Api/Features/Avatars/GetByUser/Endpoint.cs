using FastEndpoints;
using TobyMeehan.Com.Api.Requests;
using TobyMeehan.Com.Api.Security;
using TobyMeehan.Com.Services;
using IAuthorizationService = Microsoft.AspNetCore.Authorization.IAuthorizationService;

namespace TobyMeehan.Com.Api.Features.Avatars.GetByUser;

public class Endpoint : Endpoint<AuthenticatedRequest, List<AvatarResponse>>
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
        Get("/users/{UserId}/avatars");
        Policies(PolicyNames.User.Scope.Identify);
    }

    public override async Task HandleAsync(AuthenticatedRequest req, CancellationToken ct)
    {
        if (!req.TryGetUserId(out var userId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var result = await _users.GetByIdAsync(userId, cancellationToken: ct);

        await result.Match(
            user => AuthorizeAsync(user, ct),
            notFound => SendNotFoundAsync(ct));
    }

    private async Task AuthorizeAsync(IUser user, CancellationToken ct)
    {
        var authorizationResult =
            await _authorizationService.AuthorizeAsync(User, user, PolicyNames.User.Operation.Identify);

        if (authorizationResult.Succeeded)
        {
            await GetAsync(user, ct);
            return;
        }

        await SendForbiddenAsync(ct);
    }

    private async Task GetAsync(IUser user, CancellationToken ct)
    {
        var avatars = await _service
            .GetByUserAsync(user.Id, cancellationToken: ct)
            .ToListAsync(ct);
        
        await SendAsync(avatars.Select(avatar => new AvatarResponse
        {
            Id = avatar.Id.Value
        }).ToList(), cancellation: ct);
    }
}