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
        public const string CanEditDownload = "CanEditDownload";
        public const string CanEditComment = "CanEditComment";

        public static AuthorizationPolicy IsVerifiedPolicy() => new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .RequireRole("Admin", "Verified")
            .Build();

        public static AuthorizationPolicy IsAdminPolicy() => new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .RequireRole("Admin")
            .Build();

        public static AuthorizationPolicy CanEditDownloadPolicy() => new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .AddRequirements(new EditDownloadRequirement())
            .Build();

        public static AuthorizationPolicy CanEditCommentPolicy() => new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .AddRequirements(new EditCommentRequirement())
            .Build();
    }
}
