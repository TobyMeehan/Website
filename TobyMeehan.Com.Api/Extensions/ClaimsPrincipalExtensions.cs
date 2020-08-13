using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TobyMeehan.Com.Api.Authorization;

namespace TobyMeehan.Com.Api.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static bool HasScope(this ClaimsPrincipal user, string scope)
        {
            return user.HasClaim(Scopes.ClaimType, scope);
        }
    }
}
