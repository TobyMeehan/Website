using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using TobyMeehan.Com.Api.Security;
using TobyMeehan.Com.Builders.User;
using TobyMeehan.Com.Services;
using IAuthorizationService = Microsoft.AspNetCore.Authorization.IAuthorizationService;

namespace TobyMeehan.Com.Api.Features.Users.Password;

public class Endpoint : Endpoint<Request>
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
        Put("/users/{UserId}/password");
        Policies(PolicyNames.User.Scope.Password);
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        if (!req.TryGetUserId(out var userId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var result = 
            await _service.GetByCredentialsAsync(userId, req.CurrentPassword, cancellationToken: ct);

        await result.Match(
            user => AuthorizeAsync(user, req, ct),
            invalid => SendUnauthorizedAsync(ct),
            notFound => SendNotFoundAsync(ct));
    }

    private async Task AuthorizeAsync(IUser user, Request req, CancellationToken ct)
    {
        var authorizationResult = 
            await _authorizationService.AuthorizeAsync(User, user, PolicyNames.User.Operation.Password);

        if (authorizationResult.Succeeded)
        {
            await UpdateAsync(user.Id, req, ct);
            return;
        }

        await SendForbiddenAsync(ct);
    }
    
    private async Task UpdateAsync(Id<IUser> id, Request req, CancellationToken ct)
    {
        var result = 
            await _service.UpdateAsync(id, new UpdateUserBuilder().WithPassword(req.NewPassword), ct);

        await result.Match(
            user => SendOkAsync(ct),
            notFound => SendNotFoundAsync(ct));
    }
}