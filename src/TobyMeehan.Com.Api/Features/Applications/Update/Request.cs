using TobyMeehan.Com.Api.Requests;

namespace TobyMeehan.Com.Api.Features.Applications.Update;

public class Request : AuthenticatedRequest
{
    public string Id { get; set; } = default!;
    
    public Optional<string> Name { get; set; }
    
    public Optional<string?> Description { get; set; }
}