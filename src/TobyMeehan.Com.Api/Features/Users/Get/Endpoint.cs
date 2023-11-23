using FastEndpoints;
using OpenIddict.Abstractions;
using TobyMeehan.Com.Api.Requests;
using TobyMeehan.Com.Api.Security;
using TobyMeehan.Com.Services;
using IAuthorizationService = Microsoft.AspNetCore.Authorization.IAuthorizationService;

namespace TobyMeehan.Com.Api.Features.Users.Get;

public class Endpoint : Endpoint<AuthenticatedRequest, UserResponse>
{
    private readonly IUserService _service;
    private readonly IAuthorizationService _authorizationService;

    public Endpoint(IUserService service, IAuthorizationService authorizationService)
    {
        _service = service;
        _authorizationService = authorizationService;
    }
    
    public override void Configure()
    {
        Get("/users/{UserId}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(AuthenticatedRequest req, CancellationToken ct)
    {
        if (!req.TryGetUserId(out var userId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var result = await _service.GetByIdAsync(userId, cancellationToken: ct);

        await result.Match(
            user => AuthorizeAsync(user, ct),
            notFound => SendNotFoundAsync(ct));
    }

    private async Task AuthorizeAsync(IUser user, CancellationToken ct)
    {
        var authorizationResult =
            await _authorizationService.AuthorizeAsync(User, user, PolicyNames.User.Operation.Read);

        if (authorizationResult.Succeeded)
        {
            await GetAsync(user, ct);
            return;
        }

        await SendForbiddenAsync(ct);
    }
    
    private async Task GetAsync(IUser user, CancellationToken ct)
    {
        var response = new UserResponse
        {
            Id = user.Id.Value,
            Username = user.Username,
            DisplayName = user.DisplayName,
            Description = user.Description
        };

        var authorizationResult =
            await _authorizationService.AuthorizeAsync(User, null, PolicyNames.User.Scope.Identify);

        if (authorizationResult.Succeeded)
        {
            response.Balance = user.Balance;
        }

        await SendAsync(response, cancellation: ct);
    }
}