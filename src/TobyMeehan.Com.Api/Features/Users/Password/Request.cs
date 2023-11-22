using TobyMeehan.Com.Api.Requests;

namespace TobyMeehan.Com.Api.Features.Users.Password;

public class Request : AuthenticatedRequest
{
    public required string CurrentPassword { get; set; }
    public required string NewPassword { get; set; }
}