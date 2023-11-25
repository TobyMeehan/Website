using TobyMeehan.Com.Api.Requests;

namespace TobyMeehan.Com.Api.Features.Users.Avatar;

public class Request : AuthenticatedRequest
{
    public string? AvatarId { get; set; } = default!;
}