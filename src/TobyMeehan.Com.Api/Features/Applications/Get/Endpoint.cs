using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using TobyMeehan.Com.Api.Requests;
using TobyMeehan.Com.Api.Security;
using TobyMeehan.Com.Services;
using IAuthorizationService = Microsoft.AspNetCore.Authorization.IAuthorizationService;

namespace TobyMeehan.Com.Api.Features.Applications.Get;

public class Endpoint : Endpoint<AuthenticatedRequest, ApplicationResponse>
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
        Get("/applications/{ApplicationId}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(AuthenticatedRequest req, CancellationToken ct)
    {
        if (!req.TryGetApplicationId(out var applicationId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }
        
        var result = await _service.GetByIdAsync(applicationId, cancellationToken: ct);

        await result.Match(
            application => GetAsync(application, ct),
            notFound => SendNotFoundAsync(ct));
    }

    private async Task GetAsync(IApplication application, CancellationToken ct)
    {
        await SendAsync(new ApplicationResponse
        {
            Id = application.Id.Value,
            AuthorId = application.AuthorId.Value,
            Name = application.Name,
            Description = application.Description
        }, cancellation: ct);
    }
}