using TobyMeehan.Com.Api.Requests;

namespace TobyMeehan.Com.Api.Features.Avatars.Get;

public class Request : AuthenticatedRequest
{
    public string Id { get; set; } = default!;
}