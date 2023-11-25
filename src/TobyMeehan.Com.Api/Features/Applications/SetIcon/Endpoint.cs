using FastEndpoints;
using TobyMeehan.Com.Api.CollectionAuthorization;
using TobyMeehan.Com.Api.Requests;
using TobyMeehan.Com.Api.Security;
using TobyMeehan.Com.Builders;
using TobyMeehan.Com.Builders.Application;
using TobyMeehan.Com.Services;
using IAuthorizationService = Microsoft.AspNetCore.Authorization.IAuthorizationService;

namespace TobyMeehan.Com.Api.Features.Applications.SetIcon;

public class Endpoint : Endpoint<Request>
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
        Verbs(Http.PUT, Http.DELETE);
        Routes("/applications/{ApplicationId}/icon", "/users/{UserId}/applications/{ApplicationId}/icon");
        Policies(PolicyNames.Application.Scope.Update);
        AllowFileUploads();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        if (!req.TryGetApplicationId(out var applicationId))
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var result = req.TryGetUserId(out var userId)
            ? await _service.GetByIdAndAuthorAsync(applicationId, userId, cancellationToken: ct)
            : await _service.GetByIdAsync(applicationId, cancellationToken: ct);

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
            await SetIconAsync(application, req, ct);
            return;
        }

        await SendForbiddenAsync(ct);
    }

    private async Task SetIconAsync(IApplication application, Request req, CancellationToken ct)
    {
        if (HttpMethod == Http.DELETE)
        {
            await _service.UpdateAsync(application.Id, new UpdateApplicationBuilder().WithIcon(null), ct);
            await SendNoContentAsync(ct);
            return;
        }

        if (req.Icon is null)
        {
            ThrowError(x => x.Icon, "New icon not provided.");
        }
        
        await _service.UpdateAsync(application.Id, new UpdateApplicationBuilder()
                
            .WithIcon(new FileUploadBuilder()
                .WithFilename(req.Icon.FileName)
                .WithContentType(MediaType.Parse(req.Icon.ContentType))
                .WithSize(req.Icon.Length)
                .WithStream(req.Icon.OpenReadStream())), 
            
            cancellationToken: ct);

        await SendOkAsync(ct);
    }
}