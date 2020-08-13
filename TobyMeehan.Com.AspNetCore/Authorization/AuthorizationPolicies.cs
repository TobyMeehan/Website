using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace TobyMeehan.Com.AspNetCore.Authorization
{
    public class AuthorizationPolicies
    {
        public const string IsVerified = "IsVerified";
        public const string IsAdmin = "IsAdmin";

        public const string CanEditDownload = nameof(CanEditDownload);
        public const string CanEditComment = nameof(CanEditComment);
        public const string CanEditApplication = nameof(CanEditApplication);

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
            .AddRequirements(new UpdateOperationAuthorizationRequirement())
            .Build();

        public static AuthorizationPolicy CanEditCommentPolicy() => new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .AddRequirements(new UpdateOperationAuthorizationRequirement())
            .Build();

        public static AuthorizationPolicy CanEditApplicationPolicy() => new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .AddRequirements(new UpdateOperationAuthorizationRequirement())
            .Build();
    }
}
