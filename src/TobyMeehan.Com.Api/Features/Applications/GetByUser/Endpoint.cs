using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using TobyMeehan.Com.Api.CollectionAuthorization;
using TobyMeehan.Com.Api.Requests;
using TobyMeehan.Com.Api.Security;
using TobyMeehan.Com.Services;
using IAuthorizationService = Microsoft.AspNetCore.Authorization.IAuthorizationService;

namespace TobyMeehan.Com.Api.Features.Applications.GetByUser;

public class Endpoint : Endpoint<AuthenticatedRequest, List<ApplicationResponse>>
{
    private readonly IApplicationService _service;
    private readonly IUserService _users;
    private readonly IAuthorizationService _authorizationService;

    public Endpoint(IApplicationService service, IUserService users, IAuthorizationService authorizationService)
    {
        _service = service;
        _users = users;
        _authorizationService = authorizationService;
    }
    
    public override void Configure()
    {
        Get("/users/{UserId}/applications");
        Policies(PolicyNames.Application.Scope.Read);
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
            async user =>
            {
                var applications = await _service
                    .GetByAuthorAsync(userId, cancellationToken: ct)
                    .ToListAsync(ct);

                await AuthorizeAsync(applications, user, ct);
            },
            notFound => SendNotFoundAsync(ct));
    }

    private async Task AuthorizeAsync(IReadOnlyCollection<IApplication> applications, IUser user, CancellationToken ct)
    {
        var authorizationResult = 
            await _authorizationService.AuthorizeAsync(User, applications, user, PolicyNames.Application.Operation.Read);

        if (authorizationResult.Succeeded)
        {
            await GetAsync(authorizationResult.AuthorizedResources, ct);
            return;
        }

        await SendForbiddenAsync(ct);
    }

    private async Task GetAsync(IEnumerable<IApplication> applications, CancellationToken ct)
    {
        await SendAsync(applications.Select(application =>
            new ApplicationResponse
            {
                Id = application.Id.Value,
                AuthorId = application.AuthorId.Value,
                Name = application.Name,
                Description = application.Description
            }).ToList(), cancellation: ct);
    }
}