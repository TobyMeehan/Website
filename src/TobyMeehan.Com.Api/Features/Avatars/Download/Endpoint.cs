using FastEndpoints;
using TobyMeehan.Com.Api.Security;
using TobyMeehan.Com.Api.Services.Icons;
using TobyMeehan.Com.Services;
using IAuthorizationService = Microsoft.AspNetCore.Authorization.IAuthorizationService;

namespace TobyMeehan.Com.Api.Features.Avatars.Download;

public class Endpoint : Endpoint<Request>
{
    private readonly IAvatarService _service;
    private readonly IUserService _users;
    private readonly IIconService _iconService;
    private readonly IAuthorizationService _authorizationService;

    public Endpoint(IAvatarService service, IUserService users, IIconService iconService,
        IAuthorizationService authorizationService)
    {
        _service = service;
        _users = users;
        _iconService = iconService;
        _authorizationService = authorizationService;
    }

    public override void Configure()
    {
        Get("/users/{UserId}/avatars/{AvatarId}/avatar{Extension}", "/users/{UserId}/avatar{Extension}");
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
            user => AuthorizeAsync(user, req, ct),
            notFound => SendNotFoundAsync(ct));
    }

    private async Task AuthorizeAsync(IUser user, Request req, CancellationToken ct)
    {
        string policy = req.AvatarId == user.Avatar?.Id.Value
            ? PolicyNames.User.Operation.Read
            : PolicyNames.User.Operation.Identify;

        var authorizationResult =
            await _authorizationService.AuthorizeAsync(User, user, policy);

        if (authorizationResult.Succeeded)
        {
            await DownloadAsync(user, req, ct);
            return;
        }

        await SendForbiddenAsync(ct);
    }

    private async Task DownloadAsync(IUser user, Request req, CancellationToken ct)
    {
        var avatar = user.Avatar;

        if (req.AvatarId is not null)
        {
            var result =
                await _service.GetByIdAndUserAsync(new Id<IAvatar>(req.AvatarId), user.Id, cancellationToken: ct);

            avatar = result.Match(
                found => found,
                notFound =>
                {
                    ThrowError(x => x.AvatarId, "Could not find specified avatar.", 404);
                    return user.Avatar;
                });
        }

        var contentType = avatar?.ContentType ?? MediaType.Parse("image/png");
        
        if (req.Extension != contentType.Extension)
        {
            AddError(x => x.Extension, "Avatar filename is invalid.");
            ThrowIfAnyErrors();
        }

        HttpContext.MarkResponseStart();
        HttpContext.Response.StatusCode = 200;
        HttpContext.Response.ContentType = contentType.ToString();
        
        switch (avatar)
        {
            case null:
                var options = new Dictionary<string, string>
                {
                    ["seed"] = user.Username,
                    ["backgroundColor"] = "ffffff"
                };
                
                await _iconService.DownloadAsync("identicon", contentType.Extension.TrimStart('.'), options, HttpContext.Response.Body, ct);
                break;
            
            default:
                await _service.DownloadAsync(avatar, HttpContext.Response.Body, ct);
                break;
        }
    }
}