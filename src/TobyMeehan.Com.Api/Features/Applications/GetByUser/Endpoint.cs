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
        Get("/users/@me/applications");
        Policies(ScopeNames.Applications.Read);
    }

    public override async Task HandleAsync(AuthenticatedRequest req, CancellationToken ct)
    {
        var applications = _service.GetByAuthorAsync(req.UserId, cancellationToken: ct);

        var response = new List<ApplicationResponse>();

        await foreach (var application in applications)
        {
            var authorizationResult =
                await _authorizationService.AuthorizeAsync(User, application, OperationRequirements.Read);
            
            if (authorizationResult.Succeeded)
            {
                response.Add(new ApplicationResponse
                {
                    Id = application.Id.Value,
                    AuthorId = application.AuthorId.Value,
                    Name = application.Name,
                    Description = application.Description
                });
            }
        }

        await SendAsync(response, cancellation: ct);
    }
}