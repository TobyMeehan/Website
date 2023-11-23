using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using TobyMeehan.Com.Api.Requests;
using TobyMeehan.Com.Api.Security;
using TobyMeehan.Com.Services;
using IAuthorizationService = Microsoft.AspNetCore.Authorization.IAuthorizationService;

namespace TobyMeehan.Com.Api.Features.Users.Delete;

public class Endpoint : Endpoint<AuthenticatedRequest>
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
        Delete("/users/{UserId}");
        Policies(PolicyNames.User.Scope.Delete);
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

    public async Task AuthorizeAsync(IUser user, CancellationToken ct)
    {
        var authorizationResult = 
            await _authorizationService.AuthorizeAsync(User, user, PolicyNames.User.Operation.Delete);

        if (authorizationResult.Succeeded)
        {
            await DeleteAsync(user, ct);
            return;
        }

        await SendForbiddenAsync(ct);
    }
    
    public async Task DeleteAsync(IUser user, CancellationToken ct)
    {
        var result = await _service.DeleteAsync(user.Id, ct);

        await result.Match(
            success => SendNoContentAsync(ct),
            notFound => SendNotFoundAsync(ct));
    }
}