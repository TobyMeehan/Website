using FastEndpoints;
using TobyMeehan.Com.Api.Features.Avatars;
using TobyMeehan.Com.Api.Security;
using TobyMeehan.Com.Builders.User;
using TobyMeehan.Com.Services;
using IAuthorizationService = Microsoft.AspNetCore.Authorization.IAuthorizationService;

namespace TobyMeehan.Com.Api.Features.Users.Avatar;

public class Endpoint : Endpoint<Request, AvatarResponse>
{
    private readonly IUserService _service;
    private readonly IAvatarService _avatars;
    private readonly IAuthorizationService _authorizationService;

    public Endpoint(IUserService service, IAvatarService avatars, IAuthorizationService authorizationService)
    {
        _service = service;
        _avatars = avatars;
        _authorizationService = authorizationService;
    }

    public override void Configure()
    {
        Verbs(Http.PUT, Http.DELETE);
        Routes("/users/{UserId}/avatar");
        Policies(PolicyNames.User.Scope.Update);
        AllowFileUploads();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        if (!req.TryGetUserId(out var userId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var result = await _service.GetByIdAsync(userId, cancellationToken: ct);

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
            await SetAvatarAsync(user, req, ct);
            return;
        }

        await SendForbiddenAsync(ct);
    }

    private async Task SetAvatarAsync(IUser user, Request req, CancellationToken ct)
    {
        if (HttpMethod == Http.DELETE)
        {
            await _service.UpdateAsync(user.Id, new UpdateUserBuilder().WithAvatar(null), ct);
            await SendNoContentAsync(ct);
            return;
        }

        if (req.AvatarId is null)
        {
            ThrowError(x => x.AvatarId, "Avatar ID is required.");
        }
        
        var result = 
            await _avatars.GetByIdAndUserAsync(new Id<IAvatar>(req.AvatarId), user.Id, cancellationToken: ct);

        await result.Match(
            async avatar =>
            {
                await _service.UpdateAsync(user.Id, new UpdateUserBuilder().WithAvatar(avatar.Id), ct);
                await SendOkAsync(ct);
            },
            notFound =>
            {
                ThrowError(x => x.AvatarId, "User does not have an avatar with the specified ID.", 404);
                return Task.CompletedTask;
            });
    }
}