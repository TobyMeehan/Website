using TobyMeehan.Com.Api.Requests;

namespace TobyMeehan.Com.Api.Features.Avatars.Create;

public class Request : AuthenticatedRequest
{
    public required IFormFile Avatar { get; set; }
}