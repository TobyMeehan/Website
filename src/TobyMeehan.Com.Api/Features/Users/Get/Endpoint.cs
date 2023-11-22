using FastEndpoints;
using OpenIddict.Abstractions;
using TobyMeehan.Com.Api.Requests;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Api.Features.Users.Get;

public class Endpoint : Endpoint<AuthenticatedRequest, UserResponse>
{
    private readonly IUserService _service;

    public Endpoint(IUserService service)
    {
        _service = service;
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
            user => GetAsync(user, ct),
            notFound => SendNotFoundAsync(ct));
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

        if (User.HasScope(ScopeNames.Account.Identify))
        {
            response.Balance = user.Balance;
        }

        await SendAsync(response, cancellation: ct);
    }
}