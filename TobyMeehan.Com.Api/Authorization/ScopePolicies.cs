using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Api.Authorization
{
    public class ScopePolicies
    {
        public const string HasApplicationsScope = nameof(HasApplicationsScope);
        public const string HasConnectionsScope = nameof(HasConnectionsScope);
        public const string HasDownloadsScope = nameof(HasDownloadsScope);
        public const string HasIdentifyScope = nameof(HasIdentifyScope);
        public const string HasTransactionsScope = nameof(HasTransactionsScope);

        public static AuthorizationPolicy HasApplicationsScopePolicy() => new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .AddRequirements(new ScopeAuthorizationRequirement(Scopes.Applications))
            .Build();

        public static AuthorizationPolicy HasConnectionsScopePolicy() => new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .AddRequirements(new ScopeAuthorizationRequirement(Scopes.Connections))
            .Build();

        public static AuthorizationPolicy HasDownloadsScopePolicy() => new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .AddRequirements(new ScopeAuthorizationRequirement(Scopes.Downloads))
            .Build();

        public static AuthorizationPolicy HasIdentifyScopePolicy() => new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .AddRequirements(new ScopeAuthorizationRequirement(Scopes.Identify))
            .Build();

        public static AuthorizationPolicy HasTransactionsScopePolicy() => new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .AddRequirements(new ScopeAuthorizationRequirement(Scopes.Transactions))
            .Build();
    }
}
