using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static TobyMeehan.Com.Authorization.AuthorizationPolicies;

namespace TobyMeehan.Com.Authorization
{
    public static class AuthorizationOptionsExtensions
    {
        public static void AddAuthorizationPolicies(this AuthorizationOptions options)
        {
            options.AddPolicy(IsVerified, IsVerifiedPolicy());
            options.AddPolicy(IsAdmin, IsAdminPolicy());
            options.AddPolicy(CanEditDownload, CanEditDownloadPolicy());
        }
    }
}
