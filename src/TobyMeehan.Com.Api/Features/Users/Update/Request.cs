using TobyMeehan.Com.Api.Requests;

namespace TobyMeehan.Com.Api.Features.Users.Update;

public class Request : AuthenticatedRequest
{
    public Optional<string> Username { get; set; }
    public Optional<string> DisplayName { get; set; }
    public Optional<string?> Description { get; set; }
}