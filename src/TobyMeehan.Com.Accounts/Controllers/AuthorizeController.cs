using System.Collections.Immutable;
using System.Security.Claims;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using TobyMeehan.Com.Accounts.Extensions;
using TobyMeehan.Com.Accounts.Models.Authorize;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Accounts.Controllers;

public class AuthorizeController : Controller
{
    private readonly IUserService _users;
    private readonly IOpenIddictApplicationManager _applicationManager;
    private readonly IOpenIddictAuthorizationManager _authorizationManager;

    public AuthorizeController(
        IUserService users,
        IOpenIddictApplicationManager applicationManager,
        IOpenIddictAuthorizationManager authorizationManager)
    {
        _users = users;
        _applicationManager = applicationManager;
        _authorizationManager = authorizationManager;
    }

    [HttpGet("/oauth/authorize")]
    public async Task<IActionResult> AuthorizeAsync(CancellationToken ct)
    {
        var request = HttpContext.GetOpenIddictServerRequest() ??
                      throw new InvalidOperationException("Could not retrieve OpenID Connect request.");

        if (User.Identity?.IsAuthenticated is not true)
        {
            return Challenge();
        }

        var userResult = await _users.GetByIdAsync(User.Id(), cancellationToken: ct);

        var user = userResult.Match(
            user => user,
            notFound => throw new InvalidOperationException("Could not retrieve user details."));

        object application = await _applicationManager.FindByClientIdAsync(request.ClientId ?? string.Empty, ct) ??
                             throw new InvalidOperationException("Could not retrieve application details.");

        var permanentAuthorizations = await GetPermanentAuthorizationsAsync(
            subject: user.Id.Value,
            client: (await _applicationManager.GetIdAsync(application, ct))!,
            scopes: request.GetScopes(),
            ct);

        switch (await _applicationManager.GetConsentTypeAsync(application, cancellationToken: ct))
        {
            case OpenIddictConstants.ConsentTypes.Implicit:
            case OpenIddictConstants.ConsentTypes.Explicit when permanentAuthorizations.Any() && !request.HasPrompt(OpenIddictConstants.Prompts.Consent):

                return await AuthorizeAsync(
                    subject: user.Id.Value,
                    client: (await _applicationManager.GetIdAsync(application, ct))!,
                    scopes: request.GetScopes(),
                    authorization: permanentAuthorizations.LastOrDefault(),
                    ct);
            
            default: return View(new AuthorizeViewModel
            {
                Client = new ApplicationViewModel
                {
                    Name = await _applicationManager.GetDisplayNameAsync(application, ct)
                },
                Owner = user,
                ReturnUrl = HttpContext.Request.Path + HttpContext.Request.QueryString,
                Scopes = request.GetScopes()
            });
        }
    }

    [Authorize, FormValueRequired("submit.Authorize")]
    [HttpPost("/oauth/authorize"), ValidateAntiForgeryToken]
    public async Task<IActionResult> AcceptAsync(CancellationToken ct)
    {
        var request = HttpContext.GetOpenIddictServerRequest() ??
                      throw new InvalidOperationException("Could not retrieve OpenID Connect request.");
        
        var userResult = await _users.GetByIdAsync(User.Id(), cancellationToken: ct);

        var user = userResult.Match(
            user => user,
            notFound => throw new InvalidOperationException("Could not retrieve user details."));

        object application = await _applicationManager.FindByClientIdAsync(request.ClientId!, ct) ??
                             throw new InvalidOperationException("Could not retrieve application details.");

        var permanentAuthorizations = await GetPermanentAuthorizationsAsync(
            subject: user.Id.Value,
            client: (await _applicationManager.GetIdAsync(application, ct))!,
            scopes: request.GetScopes(),
            ct);

        return await AuthorizeAsync(
            subject: user.Id.Value,
            client: (await _applicationManager.GetIdAsync(application, ct))!,
            scopes: request.GetScopes(),
            authorization: permanentAuthorizations.LastOrDefault(),
            ct);
    }

    [Authorize, FormValueRequired("submit.Cancel")]
    [HttpPost("/oauth/authorize"), ValidateAntiForgeryToken]
    public IActionResult Cancel() => Forbid(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    
    private async Task<List<object>> GetPermanentAuthorizationsAsync(
        string subject, 
        string client, 
        ImmutableArray<string> scopes, 
        CancellationToken ct)
    {
        return await _authorizationManager.FindAsync(
            subject: subject,
            client: client,
            status: OpenIddictConstants.Statuses.Valid,
            type: OpenIddictConstants.AuthorizationTypes.Permanent,
            scopes,
            cancellationToken: ct).ToListAsync(cancellationToken: ct);
    }
    
    private async Task<IActionResult> AuthorizeAsync(
        string subject, 
        string client, 
        ImmutableArray<string> scopes, 
        object? authorization, 
        CancellationToken ct)
    {
        var identity = new ClaimsIdentity(
            authenticationType: TokenValidationParameters.DefaultAuthenticationType,
            nameType: OpenIddictConstants.Claims.Name,
            roleType: OpenIddictConstants.Claims.Role);

        identity.SetClaim(OpenIddictConstants.Claims.Subject, subject, OpenIddictConstants.Destinations.AccessToken);

        identity.SetScopes(scopes);
        
        authorization ??= await _authorizationManager.CreateAsync(
            identity: identity,
            subject: subject,
            client: client,
            type: OpenIddictConstants.AuthorizationTypes.Permanent,
            scopes: identity.GetScopes(),
            cancellationToken: ct);

        identity.SetAuthorizationId(await _authorizationManager.GetIdAsync(authorization, ct));

        return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }
}