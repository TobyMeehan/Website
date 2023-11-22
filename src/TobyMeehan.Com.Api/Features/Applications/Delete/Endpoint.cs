using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using TobyMeehan.Com.Api.Requests;
using TobyMeehan.Com.Api.Security;
using TobyMeehan.Com.Services;
using IAuthorizationService = Microsoft.AspNetCore.Authorization.IAuthorizationService;

namespace TobyMeehan.Com.Api.Features.Applications.Delete;

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

        await result.Match(
            application => AuthorizeAsync(application, ct),
            notFound => SendNotFoundAsync(ct));
    }

    private async Task AuthorizeAsync(IApplication application, CancellationToken ct)
    {
        var authorizationResult =
            await _authorizationService.AuthorizeAsync(User, application, OperationRequirements.Delete);

        if (authorizationResult.Succeeded)
        {
            await DeleteAsync(application, ct);
            return;
        }

        await SendForbiddenAsync(ct);
    }

    private async Task DeleteAsync(IApplication application, CancellationToken ct)
    {
        var result = await _service.DeleteAsync(application.Id, ct);

        await result.Match(
            success => SendNoContentAsync(ct),
            notFound => SendNotFoundAsync(ct));
    }
}