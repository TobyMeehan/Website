using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.Json;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using OpenIddict.Core;
using TobyMeehan.Com.Accounts.Models;
using TobyMeehan.Com.Accounts.Models.OpenId;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Accounts.OpenId;

public class OpenIdApplicationManager : OpenIddictApplicationManager<OpenIdApplication>
{
    private readonly IApplicationService _applications;

    public OpenIdApplicationManager(
        IApplicationService applications, 
        IOpenIddictApplicationCache<OpenIdApplication> cache, 
        ILogger<OpenIdApplicationManager> logger, 
        IOptionsMonitor<OpenIddictCoreOptions> options, 
        IOpenIddictApplicationStoreResolver resolver) 
        : base(cache, logger, options, resolver)
    {
        _applications = applications;
    }

    public override async ValueTask<bool> ValidateClientSecretAsync(OpenIdApplication application, string secret,
        CancellationToken cancellationToken = new())
    {
        var result = await _applications.GetByCredentialsAsync(application.Id, secret,
            cancellationToken: cancellationToken);

        return result.Match(
            found => true,
            invalid => false,
            notFound => false);
    }
}