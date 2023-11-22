using System.Security.Claims;
using OpenIddict.Abstractions;

namespace TobyMeehan.Com.Api.Security;

public static class ClaimsPrincipalExtensions
{
    public static Id<IUser> GetSubject(this ClaimsPrincipal user)
    {
        string? claim = user.GetClaim(OpenIddictConstants.Claims.Subject);

        if (claim is null)
        {
            throw new InvalidOperationException("Subject claim not set.");
        }
        
        return new Id<IUser>(claim);
    }

    public static Id<IApplication> GetClientId(this ClaimsPrincipal user)
    {
        string? claim = user.GetClaim(OpenIddictConstants.Claims.ClientId);

        if (claim is null)
        {
            throw new InvalidOperationException("Client ID claim not set.");
        }

        return new Id<IApplication>(claim);
    }
}