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
        public const string IsAdmin = "IsAdmin";
        public const string EditDownload = "EditDownload";

        public static AuthorizationPolicy IsVerifiedPolicy()
        {
            return new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireRole("Admin", "Verified")
                .Build();
        }

        public static AuthorizationPolicy IsAdminPolicy()
        {
            return new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireRole("Admin")
                .Build();
        }

        public static AuthorizationPolicy EditDownloadPolicy()
        {
            return new AuthorizationPolicyBuilder()
                .AddRequirements(new DownloadAuthorRequirement())
                .Build();
        }
    }
}
