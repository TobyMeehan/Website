using FastEndpoints;
using TobyMeehan.Com.Api.Security;
using TobyMeehan.Com.Builders.User;
using TobyMeehan.Com.Services;
using IAuthorizationService = Microsoft.AspNetCore.Authorization.IAuthorizationService;

namespace TobyMeehan.Com.Api.Features.Avatars.Delete;

public class Endpoint : Endpoint<Request>
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
        Delete("/users/{UserId}/avatars/{AvatarId}");
        Policies(PolicyNames.User.Scope.Update);
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
            await _authorizationService.AuthorizeAsync(User, user, PolicyNames.User.Operation.Update);

        if (authorizationResult.Succeeded)
        {
            await DeleteAsync(user, req, ct);
            return;
        }

        await SendForbiddenAsync(ct);
    }

    private async Task DeleteAsync(IUser user, Request req, CancellationToken ct)
    {
        var result = 
            await _service.GetByIdAndUserAsync(new Id<IAvatar>(req.AvatarId), user.Id, cancellationToken: ct);

        await result.Match(
            async avatar =>
            {
                if (avatar.Id == user.Avatar?.Id)
                {
                    await _users.UpdateAsync(user.Id, new UpdateUserBuilder().WithAvatar(null), ct);
                }

                await _service.DeleteAsync(avatar.Id, ct);
                await SendNoContentAsync(ct);
            },
            notFound => SendNotFoundAsync(ct));
    }
}