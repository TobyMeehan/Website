using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace TobyMeehan.Com.Api.Security.Transaction;

public class TransactionOperationPolicies
{
    public static readonly AuthorizationPolicy Create = new AuthorizationPolicyBuilder()
        .AddRequirements(OperationRequirements.Create)
        .Build();

    public static readonly AuthorizationPolicy Transfer = new AuthorizationPolicyBuilder()
        .AddRequirements(new OperationAuthorizationRequirement { Name = "Transfer" })
        .Build();

    public static readonly AuthorizationPolicy Read = new AuthorizationPolicyBuilder()
        .AddRequirements(OperationRequirements.Read)
        .Build();
}