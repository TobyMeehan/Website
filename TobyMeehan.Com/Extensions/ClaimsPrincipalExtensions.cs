using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string Username(this ClaimsPrincipal user) 
            => user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;

        public static string Id(this ClaimsPrincipal user)
            => user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
    }
}
