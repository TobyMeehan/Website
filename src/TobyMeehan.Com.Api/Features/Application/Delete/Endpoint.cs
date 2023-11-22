using FastEndpoints;
using TobyMeehan.Com.Api.Requests;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Api.Features.Application.Delete;

public class Endpoint : Endpoint<IdRequest>
{
    private readonly IApplicationService _service;

    public Endpoint(IApplicationService service)
    {
        _service = service;
    }
    
    public override void Configure()
    {
        Delete("/applications/{Id}");
        Policies(ScopeNames.Applications.Delete);
    }

    public override async Task HandleAsync(IdRequest req, CancellationToken ct)
    {
        var result = await _service.DeleteAsync(new Id<IApplication>(req.Id), ct);

        await result.Match(
            success => SendNoContentAsync(ct),
            notFound => SendNotFoundAsync(ct));
    }
}