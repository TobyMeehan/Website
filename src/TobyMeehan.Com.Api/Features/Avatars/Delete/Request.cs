using TobyMeehan.Com.Api.Requests;

namespace TobyMeehan.Com.Api.Features.Avatars.Delete;

public class Request : AuthenticatedRequest
{
    public string AvatarId { get; set; } = default!;
}