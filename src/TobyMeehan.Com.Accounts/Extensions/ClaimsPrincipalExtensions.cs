using System.Security.Claims;

namespace TobyMeehan.Com.Accounts.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Id<IUser> Id(this ClaimsPrincipal user)
    {
        var claim = user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

        if (claim is null)
        {
            throw new Exception("User does not have ID claim.");
        }

        return new Id<IUser>(claim.Value);
    }
}