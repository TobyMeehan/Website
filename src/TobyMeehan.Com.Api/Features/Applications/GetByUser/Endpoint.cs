using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using TobyMeehan.Com.Api.Requests;
using TobyMeehan.Com.Api.Security;
using TobyMeehan.Com.Services;
using IAuthorizationService = Microsoft.AspNetCore.Authorization.IAuthorizationService;

namespace TobyMeehan.Com.Api.Features.Applications.GetByUser;

public class Endpoint : Endpoint<AuthenticatedRequest, List<ApplicationResponse>>
{
    private readonly IApplicationService _service;
    private readonly IAuthorizationService _authorizationService;

    public Endpoint(IApplicationService service, IAuthorizationService authorizationService)
    {
        _service = service;
        _authorizationService = authorizationService;
    }
    
    public override void Configure()
    {
        Get("/users/{UserId}/applications");
        Policies(ScopeNames.Applications.Read);
    }

    public override async Task HandleAsync(AuthenticatedRequest req, CancellationToken ct)
    {
        if (!req.TryGetUserId(out var userId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }
        
        var applications = await _service
            .GetByAuthorAsync(userId, cancellationToken: ct)
            .ToListAsync(ct);

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