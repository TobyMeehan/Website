using FastEndpoints;
using TobyMeehan.Com.Api.CollectionAuthorization;
using TobyMeehan.Com.Api.Security;
using TobyMeehan.Com.Builders.Application;
using TobyMeehan.Com.Services;
using IAuthorizationService = Microsoft.AspNetCore.Authorization.IAuthorizationService;

namespace TobyMeehan.Com.Api.Features.Applications.Create;

public class Endpoint : Endpoint<Request, ApplicationResponse>
{
    private readonly IApplicationService _service;
    private readonly IUserService _users;
    private readonly IAuthorizationService _authorizationService;

    public Endpoint(IApplicationService service, IUserService users, IAuthorizationService authorizationService)
    {
        _service = service;
        _users = users;
        _authorizationService = authorizationService;
    }
    
    public override void Configure()
    {
        Post("/users/{UserId}/applications");
        Policies(PolicyNames.Application.Scope.Create);
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        if (!req.TryGetUserId(out var userId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var result = await _users.GetByIdAsync(userId, cancellationToken: ct);

        await result.Match(
            user => AuthorizeAsync(user, req, ct),
            notFound => SendNotFoundAsync(ct));
    }

    private async Task AuthorizeAsync(IUser user, Request req, CancellationToken ct)
    {
        var authorizationResult =
            await _authorizationService.AuthorizeAsync(User, user, PolicyNames.Application.Operation.Create);

        if (authorizationResult.Succeeded)
        {
            await CreateAsync(user, req, ct);
            return;
        }

        await SendForbiddenAsync(ct);
    }

    private async Task CreateAsync(IUser user, Request req, CancellationToken ct)
    {
        var application = await _service.CreateAsync(new CreateApplicationBuilder()
            .WithName(req.Name)
            .WithAuthor(user.Id), ct);

        await SendCreatedAtAsync<Get.Endpoint>(new { ApplicationId = application.Id.Value }, responseBody:
            new ApplicationResponse
            {
                Id = application.Id.Value,
                AuthorId = application.AuthorId.Value,
                Name = application.Name,
                Description = application.Description
            }, cancellation: ct);
    }
}