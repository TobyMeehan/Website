using FastEndpoints;
using TobyMeehan.Com.Builders.Application;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Api.Features.Applications.Create;

public class Endpoint : Endpoint<Request, ApplicationResponse>
{
    private readonly IApplicationService _service;

    public Endpoint(IApplicationService service)
    {
        _service = service;
    }
    
    public override void Configure()
    {
        Post("/users/{UserId}/applications");
        Policies(ScopeNames.Applications.Create);
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        if (!req.TryGetUserId(out var userId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }
        
        var application = await _service.CreateAsync(new CreateApplicationBuilder()
            .WithName(req.Name)
            .WithAuthor(userId), ct);

        await SendCreatedAtAsync<Get.Endpoint>(new { Id = application.Id.Value }, responseBody:
            new ApplicationResponse
            {
                Id = application.Id.Value,
                AuthorId = application.AuthorId.Value,
                Name = application.Name,
                Description = application.Description
            }, cancellation: ct);
    }
}