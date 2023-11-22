using TobyMeehan.Com.Api.Requests;

namespace TobyMeehan.Com.Api.Features.Applications.Create;

public class Request : AuthenticatedRequest
{
    public required string Name { get; set; }
}