using FastEndpoints;
using TobyMeehan.Com.Builders.Application;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Api.Features.Application.Create;

public class Endpoint : Endpoint<Request, ApplicationResponse>
{
    private readonly IApplicationService _service;

    public Endpoint(IApplicationService service)
    {
        _service = service;
    }
    
    public override void Configure()
    {
        Post("/users/@me/applications");
        Policies(ScopeNames.Applications.Create);
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var application = await _service.CreateAsync(new CreateApplicationBuilder()
            .WithName(req.Name)
            .WithAuthor(req.UserId), ct);

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