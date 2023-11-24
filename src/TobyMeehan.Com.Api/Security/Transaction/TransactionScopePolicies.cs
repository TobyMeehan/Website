using Microsoft.AspNetCore.Authorization;

namespace TobyMeehan.Com.Api.Security.Transaction;

public class TransactionScopePolicies
{
    public static readonly AuthorizationPolicy Create = new AuthorizationPolicyBuilder()
        .RequireScope(ScopeNames.Transactions.Send)
        .Build();

    public static readonly AuthorizationPolicy Transfer = new AuthorizationPolicyBuilder()
        .RequireScope(ScopeNames.Transactions.Transfer)
        .Build();

    public static readonly AuthorizationPolicy Read = new AuthorizationPolicyBuilder()
        .RequireScope(ScopeNames.Transactions.Read)
        .Build();
}