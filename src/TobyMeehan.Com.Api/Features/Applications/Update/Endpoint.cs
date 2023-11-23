using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using TobyMeehan.Com.Api.Security;
using TobyMeehan.Com.Builders.Application;
using TobyMeehan.Com.Services;
using IAuthorizationService = Microsoft.AspNetCore.Authorization.IAuthorizationService;

namespace TobyMeehan.Com.Api.Features.Applications.Update;

public class Endpoint : Endpoint<Request, ApplicationResponse>
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
        Patch("/applications/{ApplicationId}");
        Policies(PolicyNames.Application.Scope.Read);
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        if (!req.TryGetApplicationId(out var applicationId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var result = await _service.GetByIdAsync(applicationId, cancellationToken: ct);

        await result.Match(
            application => AuthorizeAsync(application, req, ct),
            notFound => SendNotFoundAsync(ct));
    }

    private async Task AuthorizeAsync(IApplication application, Request req, CancellationToken ct)
    {
        var authorizationResult =
            await _authorizationService.AuthorizeAsync(User, application, PolicyNames.Application.Operation.Update);

        if (authorizationResult.Succeeded)
        {
            await UpdateAsync(application.Id, req, ct);
            return;
        }

        await SendForbiddenAsync(ct);
    }

    private async Task UpdateAsync(Id<IApplication> id, Request req, CancellationToken ct)
    {
        var result = await _service.UpdateAsync(id, new UpdateApplicationBuilder
        {
            Name = req.Name,
            Description = req.Description
        }, ct);

        await result.Match(
            application => SendAsync(new ApplicationResponse
            {
                Id = application.Id.Value,
                Name = application.Name,
                Description = application.Description,
                
                AuthorId = Optional<string>.Empty(),
            }, cancellation: ct),
            notFound => SendNotFoundAsync(ct));
    }
}