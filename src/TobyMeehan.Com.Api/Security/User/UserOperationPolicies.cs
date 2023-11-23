using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace TobyMeehan.Com.Api.Security.User;

public class UserOperationPolicies
{
    public static readonly AuthorizationPolicy Read = new AuthorizationPolicyBuilder()
        .AddRequirements(OperationRequirements.Read)
        .Build();

    public static readonly AuthorizationPolicy Identify = new AuthorizationPolicyBuilder()
        .AddRequirements(new OperationAuthorizationRequirement { Name = "Identify"})
        .Build();

    public static readonly AuthorizationPolicy Update = new AuthorizationPolicyBuilder()
        .AddRequirements(OperationRequirements.Update)
        .Build();

    public static readonly AuthorizationPolicy Password = new AuthorizationPolicyBuilder()
        .AddRequirements(new OperationAuthorizationRequirement { Name = "Password" })
        .Build();

    public static readonly AuthorizationPolicy Delete = new AuthorizationPolicyBuilder()
        .AddRequirements(OperationRequirements.Delete)
        .Build();
}