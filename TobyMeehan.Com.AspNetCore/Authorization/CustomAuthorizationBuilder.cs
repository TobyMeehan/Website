using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using TobyMeehan.Com.AspNetCore.Authorization.Applications;
using TobyMeehan.Com.AspNetCore.Authorization.Comments;
using TobyMeehan.Com.AspNetCore.Authorization.Downloads;
using TobyMeehan.Com.Data.Models;

using static TobyMeehan.Com.AspNetCore.Authorization.AuthorizationPolicies;

namespace TobyMeehan.Com.AspNetCore.Authorization
{
    public class CustomAuthorizationBuilder
    {
        public IServiceCollection Services { get; set; }

        public CustomAuthorizationBuilder(IServiceCollection services)
        {
            Services = services;

            Services.AddSingleton<IAuthorizationHandler, UserIsDownloadAuthorAuthorizationHandler>();
            Services.AddSingleton<IAuthorizationHandler, UserIsAdminAuthorizationHandler<Download>>();
            Services.AddSingleton<IAuthorizationHandler, UserIsCommentAuthorAuthorizationHandler>();
            Services.AddSingleton<IAuthorizationHandler, UserIsAdminAuthorizationHandler<Comment>>();
            Services.AddSingleton<IAuthorizationHandler, UserIsApplicationAuthorAuthorizationHandler>();

            Services.AddAuthorization(options =>
            {
                options.AddPolicy(IsVerified, IsVerifiedPolicy());
                options.AddPolicy(IsAdmin, IsAdminPolicy());

                options.AddPolicy(CanEditDownload, CanEditDownloadPolicy());
                options.AddPolicy(CanEditComment, CanEditCommentPolicy());
                options.AddPolicy(CanEditApplication, CanEditApplicationPolicy());
            });
        }
    }
}
