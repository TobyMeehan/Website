using Microsoft.AspNetCore.Authorization;
using TobyMeehan.Com.Api.Security.Transaction.Handlers;

namespace TobyMeehan.Com.Api.Security.Transaction;

public static class Extensions
{
    public static AuthorizationBuilder AddTransactionPolicies(this AuthorizationBuilder builder) => builder

        .AddPolicy(PolicyNames.Transaction.Scope.Create, TransactionScopePolicies.Create)
        .AddPolicy(PolicyNames.Transaction.Scope.Transfer, TransactionScopePolicies.Transfer)
        .AddPolicy(PolicyNames.Transaction.Scope.Read, TransactionScopePolicies.Read)

        .AddPolicy(PolicyNames.Transaction.Operation.Create, TransactionOperationPolicies.Create)
        .AddPolicy(PolicyNames.Transaction.Operation.Transfer, TransactionOperationPolicies.Transfer)
        .AddPolicy(PolicyNames.Transaction.Operation.Read, TransactionOperationPolicies.Read)
    
    ;

    public static IServiceCollection AddTransactionAuthorizationHandlers(this IServiceCollection services) => services

        .AddSingleton<IAuthorizationHandler, SameUserCreateHandler>()
        .AddSingleton<IAuthorizationHandler, RecipientOrSenderReadHandler>()
        .AddSingleton<IAuthorizationHandler, RecipientOrSenderCollectionHandler>()
    
    ;
}