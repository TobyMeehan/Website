using TobyMeehan.Com.Api.Requests;

namespace TobyMeehan.Com.Api.Features.Applications.Update;

public class Request : AuthenticatedRequest
{
    public Optional<string> Name { get; set; }
    
    public Optional<string?> Description { get; set; }
}