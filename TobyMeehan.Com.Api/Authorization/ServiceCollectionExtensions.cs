using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using static TobyMeehan.Com.Api.Authorization.ScopePolicies;

namespace TobyMeehan.Com.Api.Authorization
{
    public static class ServiceCollectionExtensions
    {
        public static void AddScopeAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(HasApplicationsScope, HasApplicationsScopePolicy());
                options.AddPolicy(HasConnectionsScope, HasConnectionsScopePolicy());
                options.AddPolicy(HasDownloadsScope, HasDownloadsScopePolicy());
                options.AddPolicy(HasIdentifyScope, HasIdentifyScopePolicy());
                options.AddPolicy(HasTransactionsScope, HasTransactionsScopePolicy());
            });
        }
    }
}
