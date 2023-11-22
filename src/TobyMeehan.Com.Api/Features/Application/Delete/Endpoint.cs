using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using TobyMeehan.Com.Api.Requests;
using TobyMeehan.Com.Api.Security;
using TobyMeehan.Com.Services;
using IAuthorizationService = Microsoft.AspNetCore.Authorization.IAuthorizationService;

namespace TobyMeehan.Com.Api.Features.Application.Delete;

public class Endpoint : Endpoint<IdRequest>
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
        Delete("/applications/{Id}");
        Policies(ScopeNames.Applications.Delete);
    }

    public override async Task HandleAsync(IdRequest req, CancellationToken ct)
    {
        var result = await _service.GetByIdAsync(new Id<IApplication>(req.Id), cancellationToken: ct);

        if (!result.IsSuccess(out var application))
        {
            await SendNotFoundAsync(ct);
            return;
        }
        
        var authorizationResult = 
            await _authorizationService.AuthorizeAsync(User, application, OperationRequirements.Delete);

        if (!authorizationResult.Succeeded)
        {
            await SendForbiddenAsync(ct);
            return;
        }
        
        await _service.DeleteAsync(new Id<IApplication>(req.Id), ct);

        await SendNoContentAsync(ct);
    }
}