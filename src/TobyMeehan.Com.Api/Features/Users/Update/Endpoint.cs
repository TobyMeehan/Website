using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using TobyMeehan.Com.Api.Security;
using TobyMeehan.Com.Builders.User;
using TobyMeehan.Com.Services;
using IAuthorizationService = Microsoft.AspNetCore.Authorization.IAuthorizationService;

namespace TobyMeehan.Com.Api.Features.Users.Update;

public class Endpoint : Endpoint<Request, UserResponse>
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
        Patch("/users/{UserId}");
        Policies(PolicyNames.User.Scope.Update);
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        if (!req.TryGetUserId(out var userId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var result = await _service.GetByIdAsync(userId, cancellationToken: ct);

        if (!result.IsSuccess(out var user))
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var authorizationResult = 
            await _authorizationService.AuthorizeAsync(User, user, PolicyNames.User.Operation.Update);

        if (!authorizationResult.Succeeded)
        {
            await SendForbiddenAsync(ct);
            return;
        }

        var updateResult = await _service.UpdateAsync(user.Id, new UpdateUserBuilder
        {
            Username = req.Username,
            DisplayName = req.DisplayName,
            Description = req.Description
        }, ct);

        await updateResult.Match(
            success => SendAsync(new UserResponse
            {
                Id = success.Id.Value,
                Username = success.Username,
                DisplayName = success.DisplayName,
                Description = success.Description
            }, cancellation: ct),
            notFound => SendNotFoundAsync(ct));
    }
}