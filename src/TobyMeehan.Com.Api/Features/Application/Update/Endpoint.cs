using FastEndpoints;
using TobyMeehan.Com.Api.Requests;
using TobyMeehan.Com.Builders.Application;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Api.Features.Application.Update;

public class Endpoint : Endpoint<Request, ApplicationResponse>
{
    private readonly IApplicationService _service;

    public Endpoint(IApplicationService service)
    {
        _service = service;
    }
    
    public override void Configure()
    {
        Patch("/applications/{Id}");
        Policies(ScopeNames.Applications.Update);
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await _service.UpdateAsync(new Id<IApplication>(req.Id), 
            new UpdateApplicationBuilder
            {
                Name = req.Name,
                Description = req.Description
            }, ct);

        await result.Match(
            
            application => SendAsync(new ApplicationResponse
            {
                Id = application.Id.Value,
                AuthorId = application.AuthorId.Value,
                Name = application.Name,
                Description = application.Description
            }, cancellation: ct),
            
            notFound => SendNotFoundAsync(ct));
    }
}