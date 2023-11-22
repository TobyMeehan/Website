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
        Patch("/applications/{Id}");
        Policies(ScopeNames.Applications.Update);
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await _service.GetByIdAsync(new Id<IApplication>(req.Id), cancellationToken: ct);

        if (!result.IsSuccess(out var application))
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var authorizationResult =
            await _authorizationService.AuthorizeAsync(User, application, OperationRequirements.Update);

        if (!authorizationResult.Succeeded)
        {
            await SendForbiddenAsync(ct);
            return;
        }
        
        await _service.UpdateAsync(new Id<IApplication>(req.Id), new UpdateApplicationBuilder
            {
                Name = req.Name,
                Description = req.Description
            }, ct);

        await SendAsync(new ApplicationResponse
        {
            Id = application.Id.Value,
            AuthorId = application.AuthorId.Value,
            Name = application.Name,
            Description = application.Description
        }, cancellation: ct);
    }
}