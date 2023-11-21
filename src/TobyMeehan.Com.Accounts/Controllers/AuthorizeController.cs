using System.Collections.Immutable;
using System.Security.Claims;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OneOf;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using TobyMeehan.Com.Accounts.Extensions;
using TobyMeehan.Com.Accounts.Models.Authorize;
using TobyMeehan.Com.Results;
using TobyMeehan.Com.Services;
using IAuthorizationService = Microsoft.AspNetCore.Authorization.IAuthorizationService;

namespace TobyMeehan.Com.Accounts.Controllers;

public class AuthorizeController : Controller
{
    private readonly IUserService _users;
    private readonly IApplicationService _applications;
    private readonly IScopeService _scopes;
    private readonly IOpenIddictAuthorizationManager _authorizationManager;

    public AuthorizeController(
        IUserService users,
        IApplicationService applications,
        IScopeService scopes,
        IOpenIddictAuthorizationManager authorizationManager)
    {
        _users = users;
        _applications = applications;
        _scopes = scopes;
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

        // Retrieve user details from principal
        
        var userResult = await _users.GetByIdAsync(User.Id(), cancellationToken: ct);

        var user = userResult.Match(
            user => user,
            notFound => throw new InvalidOperationException("Could not retrieve user details."));

        // Retrieve application details from request
        
        var applicationResult =
            await _applications.GetByIdAsync(new Id<IApplication>(request.ClientId!), cancellationToken: ct);
        
        var application = applicationResult.Match(
            application => application,
            notFound => throw new InvalidOperationException("Could not retrieve application details."));

        // Retrieve and validate scopes from request
        
        var scopeResult = await ValidateScopesAsync(request.GetScopes(), user, application, ct);

        var scopes = scopeResult.Match(
            scopes => scopes,
            forbidden => throw new InvalidOperationException("Invalid scopes in request."));
        
        // Get any existing permanent authorizations
        
        var permanentAuthorizations = await GetPermanentAuthorizationsAsync(
            subject: user.Id.Value,
            client: application.Id.Value,
            scopes: scopes,
            ct);

        // Automatically authorize if application allows and/or has been authorized before
        
        if (permanentAuthorizations.Any() && !request.HasPrompt(OpenIddictConstants.Prompts.Consent)) 
            // TODO: application has implicit role
        {
            return await AuthorizeAsync(
                subject: user.Id.Value,
                client: application.Id.Value,
                scopes: scopes,
                authorization: permanentAuthorizations.LastOrDefault(),
                ct);
        }
        
        return View(new AuthorizeViewModel
        {
            Client = new ApplicationViewModel
            {
                Name = application.Name
            },
            Owner = user,
            ReturnUrl = HttpContext.Request.Path + HttpContext.Request.QueryString,
            Scopes = scopes
        });
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

        var applicationResult =
            await _applications.GetByIdAsync(new Id<IApplication>(request.ClientId!), cancellationToken: ct);

        var application = applicationResult.Match(
            application => application,
            notFound => throw new InvalidOperationException("Could not retrieve application details."));

        var scopeResult = await ValidateScopesAsync(request.GetScopes(), user, application, ct);
        
        var scopes = scopeResult.Match(
            scopes => scopes,
            forbidden => throw new InvalidOperationException("Invalid scopes in request."));

        var permanentAuthorizations = await GetPermanentAuthorizationsAsync(
            subject: user.Id.Value,
            client: application.Id.Value,
            scopes: scopes,
            ct);

        return await AuthorizeAsync(
            subject: user.Id.Value,
            client: application.Id.Value,
            scopes: scopes,
            authorization: permanentAuthorizations.LastOrDefault(),
            ct);
    }

    [Authorize, FormValueRequired("submit.Cancel")]
    [HttpPost("/oauth/authorize"), ValidateAntiForgeryToken]
    public IActionResult Cancel() => Forbid(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    
    private async Task<List<object>> GetPermanentAuthorizationsAsync(
        string subject, 
        string client, 
        IEnumerable<IScope> scopes, 
        CancellationToken ct)
    {
        return await _authorizationManager.FindAsync(
            subject: subject,
            client: client,
            status: OpenIddictConstants.Statuses.Valid,
            type: OpenIddictConstants.AuthorizationTypes.Permanent,
            scopes: scopes.Select(x => x.Name).ToImmutableArray(),
            cancellationToken: ct).ToListAsync(cancellationToken: ct);
    }

    private async Task<OneOf<List<IScope>, Forbidden>> ValidateScopesAsync(IEnumerable<string> names, IUser user,
        IApplication application, CancellationToken ct)
    {
        var result = new List<IScope>();
        
        foreach (string name in names)
        {
            var scopeResult = await _scopes.GetByNameAsync(name, cancellationToken: ct);

            var scope = scopeResult.Match(
                scope => scope,
                notFound => throw new InvalidOperationException($"Could not retrieve scope {name}."));

            var validation = await _scopes.AuthorizeScopeAsync(scope, user, application, ct);

            bool forbid = false;

            validation.Switch(
                success => result.Add(scope),
                forbidden => forbid = true);

            if (forbid)
            {
                return new Forbidden();
            }
        }

        return result;
    }
    
    private async Task<IActionResult> AuthorizeAsync(
        string subject, 
        string client, 
        IEnumerable<IScope> scopes, 
        object? authorization, 
        CancellationToken ct)
    {
        var identity = new ClaimsIdentity(
            authenticationType: TokenValidationParameters.DefaultAuthenticationType,
            nameType: OpenIddictConstants.Claims.Name,
            roleType: OpenIddictConstants.Claims.Role);

        identity.SetClaim(OpenIddictConstants.Claims.Subject, subject);
        identity.SetClaim(OpenIddictConstants.Claims.ClientId, client);

        identity.SetScopes(scopes.Select(x => x.Name));

        identity.SetDestinations(static claim => claim.Type switch
        {
            _ => new[] { OpenIddictConstants.Destinations.AccessToken }
        });
        
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