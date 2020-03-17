using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorUI.Authorization
{
    public class Policies
    {
        public const string IsVerified = "IsVerified";

        public static AuthorizationPolicy IsVerifiedPolicy()
        {
            return new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireRole("Admin", "Verified")
                .Build();
        }
    }
}
