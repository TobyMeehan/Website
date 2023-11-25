using TobyMeehan.Com.Api.Requests;

namespace TobyMeehan.Com.Api.Features.Avatars.Download;

public class Request : AuthenticatedRequest
{
    public string? AvatarId { get; set; }
    public string Extension { get; set; } = default!;
}