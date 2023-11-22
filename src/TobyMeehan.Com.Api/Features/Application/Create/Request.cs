using TobyMeehan.Com.Api.Requests;

namespace TobyMeehan.Com.Api.Features.Application.Create;

public class Request : AuthenticatedRequest
{
    public required string Name { get; set; }
}