using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using TobyMeehan.Com.Api.Requests;
using TobyMeehan.Com.Api.Security;
using TobyMeehan.Com.Services;
using IAuthorizationService = Microsoft.AspNetCore.Authorization.IAuthorizationService;

namespace TobyMeehan.Com.Api.Features.Application.Get;

public class Endpoint : Endpoint<IdRequest, ApplicationResponse>
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
        Get("/applications/{Id}");
        AllowAnonymous();
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
            await _authorizationService.AuthorizeAsync(User, application, OperationRequirements.Read);

        if (!authorizationResult.Succeeded)
        {
            await SendForbiddenAsync(ct);
            return;
        }

        await SendAsync(new ApplicationResponse
        {
            Id = application.Id.Value,
            AuthorId = application.AuthorId.Value,
            Name = application.Name,
            Description = application.Description
        }, cancellation: ct);
    }
}