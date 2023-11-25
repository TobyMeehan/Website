using System.Net.Http.Headers;
using FastEndpoints;
using TobyMeehan.Com.Api.Requests;
using TobyMeehan.Com.Api.Security;
using TobyMeehan.Com.Api.Services.Icons;
using TobyMeehan.Com.Services;
using IAuthorizationService = Microsoft.AspNetCore.Authorization.IAuthorizationService;

namespace TobyMeehan.Com.Api.Features.Applications.DownloadIcon;

public class Endpoint : Endpoint<AuthenticatedRequest>
{
    private readonly IApplicationService _service;
    private readonly IIconService _iconService;
    private readonly IAuthorizationService _authorizationService;

    public Endpoint(IApplicationService service, IIconService iconService, IAuthorizationService authorizationService)
    {
        _service = service;
        _iconService = iconService;
        _authorizationService = authorizationService;
    }

    public override void Configure()
    {
        Get("/applications/{ApplicationId}/icon", "/users/{UserId}/applications/{ApplicationId}/icon");
        AllowAnonymous();
    }

    public override async Task HandleAsync(AuthenticatedRequest req, CancellationToken ct)
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
            application => AuthorizeAsync(application, ct),
            notFound => SendNotFoundAsync(ct));
    }

    private async Task AuthorizeAsync(IApplication application, CancellationToken ct)
    {
        var authorizationResult =
            await _authorizationService.AuthorizeAsync(User, application, PolicyNames.Application.Operation.Read);

        if (authorizationResult.Succeeded)
        {
            await DownloadAsync(application, ct);
            return;
        }

        await SendForbiddenAsync(ct);
    }

    private async Task DownloadAsync(IApplication application, CancellationToken ct)
    {
        var contentType = application.Icon?.ContentType ?? MediaType.Parse("image/png");
        
        HttpContext.MarkResponseStart();

        HttpContext.Response.StatusCode = 200;
        HttpContext.Response.ContentType = contentType.ToString();

        HttpContext.Response.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline")
        {
            FileName = $"icon{contentType.Extension}"
        }.ToString();

        switch (application.Icon)
        {
            case null:
                
                var options = new Dictionary<string, string>
                {
                    ["seed"] = application.Name
                };

                await _iconService.DownloadAsync("initials", contentType.Extension.TrimStart('.'), options,
                    HttpContext.Response.Body, ct);
                
                break;
            
            default:
                await _service.DownloadIconAsync(application, HttpContext.Response.Body, ct);
                break;
        }
    }
}