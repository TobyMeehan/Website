using FastEndpoints;
using TobyMeehan.Com.Api.Requests;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Api.Features.Application.Get;

public class Endpoint : Endpoint<IdRequest, ApplicationResponse>
{
    private readonly IApplicationService _service;

    public Endpoint(IApplicationService service)
    {
        _service = service;
    }
    
    public override void Configure()
    {
        Get("/applications/{Id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(IdRequest req, CancellationToken ct)
    {
        var result = await _service.GetByIdAsync(new Id<IApplication>(req.Id), cancellationToken: ct);
        
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