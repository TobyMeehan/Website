using Microsoft.AspNetCore.Authorization;

namespace TobyMeehan.Com.Api.Security.Download;

public class DownloadOperationPolicies
{
    public static readonly AuthorizationPolicy Create = new AuthorizationPolicyBuilder()
        .AddRequirements(OperationRequirements.Create)
        .Build();

    public static readonly AuthorizationPolicy Read = new AuthorizationPolicyBuilder()
        .AddRequirements(OperationRequirements.Read)
        .Build();

    public static readonly AuthorizationPolicy Update = new AuthorizationPolicyBuilder()
        .AddRequirements(OperationRequirements.Update)
        .Build();

    public static readonly AuthorizationPolicy Delete = new AuthorizationPolicyBuilder()
        .AddRequirements(OperationRequirements.Delete)
        .Build();
}