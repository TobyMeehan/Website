using System.Security.Claims;
using OpenIddict.Abstractions;

namespace TobyMeehan.Com.Api.Security;

public static class ClaimsPrincipalExtensions
{
    public static string? GetSubject(this ClaimsPrincipal user)
        => user.GetClaim(OpenIddictConstants.Claims.Subject);

    public static string? GetClientId(this ClaimsPrincipal user)
        => user.GetClaim(OpenIddictConstants.Claims.ClientId);
}