using FastEndpoints;
using TobyMeehan.Com.Api.Requests;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Api.Features.Application.GetByUser;

public class Endpoint : Endpoint<AuthenticatedRequest, List<ApplicationResponse>>
{
    private readonly IApplicationService _service;

    public Endpoint(IApplicationService service)
    {
        _service = service;
    }
    
    public override void Configure()
    {
        Get("/users/@me/applications");
        Policies(ScopeNames.Applications.Read);
    }

    public override async Task HandleAsync(AuthenticatedRequest req, CancellationToken ct)
    {
        var result = await _service
            .GetByAuthorAsync(req.UserId, cancellationToken: ct)
            .ToListAsync(cancellationToken: ct);
        
        await SendAsync(result.Select(application => new ApplicationResponse
        {
            Id = application.Id.Value,
            AuthorId = application.AuthorId.Value,
            Name = application.Name,
            Description = application.Description
        }).ToList(), cancellation: ct);
    }
}