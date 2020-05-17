using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Authorization
{
    public class AuthorizationPolicies
    {
        public const string IsVerified = "IsVerified";
        public const string IsAdmin = "IsAdmin";

        public static AuthorizationPolicy IsVerifiedPolicy() => new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .RequireRole("Admin", "Verified")
            .Build();

        public static AuthorizationPolicy IsAdminPolicy() => new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .RequireRole("Admin")
            .Build();
    }
}
