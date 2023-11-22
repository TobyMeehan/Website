using FastEndpoints;

namespace TobyMeehan.Com.Api.Requests;

public class AuthenticatedRequest
{
    [FromClaim(ClaimType = "subject_id_json")]
    public Id<IUser> UserId { get; set; }

    [FromClaim(ClaimType = "client_id_json")]
    public Id<IApplication> ApplicationId { get; set; }
}