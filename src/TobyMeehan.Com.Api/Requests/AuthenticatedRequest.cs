using System.Diagnostics.CodeAnalysis;
using FastEndpoints;
using OpenIddict.Abstractions;

namespace TobyMeehan.Com.Api.Requests;

public class AuthenticatedRequest
{
    [FromClaim(ClaimType = OpenIddictConstants.Claims.Subject, IsRequired = false)]
    public string? Subject { get; set; }

    [FromClaim(ClaimType = OpenIddictConstants.Claims.ClientId, IsRequired = false)]
    public string? ClientId { get; set; }

    public string? UserId { get; set; }
    public string? ApplicationId { get; set; }
    
    public bool TryGetUserId(out Id<IUser> id)
    {
        id = UserId switch
        {
            "@me" when Subject is not null => new Id<IUser>(Subject),
            null when Subject is not null => new Id<IUser>(Subject),
            not "@me" and not null => new Id<IUser>(UserId),
            _ => default
        };
        
        return id != default;
    }

    public bool TryGetApplicationId(out Id<IApplication> id)
    {
        id = ApplicationId switch
        {
            "@me" or null when ClientId is not null => new Id<IApplication>(ClientId),
            not "@me" and not null => new Id<IApplication>(ApplicationId),
            _ => default
        };

        return id != default;
    }
}