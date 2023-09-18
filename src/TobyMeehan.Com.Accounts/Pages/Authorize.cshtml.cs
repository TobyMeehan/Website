using System.Security.Cryptography;
using System.Text.Json;
using System.Web;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TobyMeehan.Com.Accounts.Extensions;
using TobyMeehan.Com.Accounts.Jwt;
using TobyMeehan.Com.Accounts.Models;
using TobyMeehan.Com.Builders;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Accounts.Pages;

public class Authorize : PageModel
{
    private readonly IApplicationService _applications;
    private readonly IUserService _users;
    private readonly IConnectionService _connections;
    private readonly ISessionService _sessions;
    private readonly IDataProtectionProvider _dataProtectionProvider;
    private readonly ITokenService _tokens;

    public Authorize(IApplicationService applications, IUserService users, IConnectionService connections, ISessionService sessions, IDataProtectionProvider dataProtectionProvider, ITokenService tokens)
    {
        _applications = applications;
        _users = users;
        _connections = connections;
        _sessions = sessions;
        _dataProtectionProvider = dataProtectionProvider;
        _tokens = tokens;
    }

    public AuthorizeErrorModel? Error { get; set; }
    
    public IApplication? Client { get; set; }
    public IUser? Owner { get; set; }
    public List<string> Scopes { get; set; } = new();

    public string? ReturnUrl { get; set; }

    private async Task<(IApplication? Client, IRedirect? Redirect, Func<IActionResult>? Error)> ValidateParametersAsync(
        AuthorizeRequestModel request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.ClientId))
        {
            return (null, null, () => PageError(OAuth.Parameters.ClientId));
        }

        var client = await _applications.GetByIdAsync(new Id<IApplication>(request.ClientId), cancellationToken);

        if (client is null)
        {
            return (null, null, () => PageError(OAuth.Parameters.ClientId));
        }

        var redirect = request.RedirectUri switch
        {
            null when client.Redirects.Count == 1 => client.Redirects.Single(),
            not null => client.Redirects.FirstOrDefault(x => x.Uri.OriginalString == request.RedirectUri),
            _ => null
        };
        
        if (redirect is null)
        {
            return (client, null, () => PageError(OAuth.Parameters.RedirectUri));
        }

        if (!new[] { OAuth.ResponseTypes.Code, OAuth.ResponseTypes.Token }.Contains(request.ResponseType))
        {
            return (client, redirect, () => RedirectToError(redirect, OAuth.Errors.UnsupportedResponseType, null, request.State));
        }

        if (!string.IsNullOrEmpty(request.Scope) && request.Scope.Split().Except(Scope.All).Any())
        {
            return (client, redirect, () => RedirectToError(redirect, OAuth.Errors.InvalidScope, null, request.State));
        }
        
        if (request is { ResponseType: OAuth.ResponseTypes.Code, CodeChallenge: not null, CodeChallengeMethod: not OAuth.Transformations.S256 })
        {
            return (client, redirect, () => RedirectToError(redirect, OAuth.Errors.InvalidRequest, "Transform algorithm not supported", request.State));
        }

        return (client, redirect, null);
    }
    
    public async Task<IActionResult> OnGetAsync(AuthorizeRequestModel request, CancellationToken cancellationToken = default)
    {
        var validation = await ValidateParametersAsync(request, cancellationToken);

        if (validation.Error is not null)
        {
            return validation.Error();
        }

        ReturnUrl = Url.Page(nameof(Authorize), new
        {
            response_type = request.ResponseType,
            client_id = request.ClientId,
            code_challenge = request.CodeChallenge,
            code_challenge_method = request.CodeChallengeMethod,
            redirect_uri = request.RedirectUri,
            scope = request.Scope,
            state = request.State
        });
        
        if (User.Identity?.IsAuthenticated is not true)
        {
            return RedirectToPage(nameof(Login), new { ReturnUrl });
        }
        
        Client = validation.Client;
        Owner = await _users.GetByIdAsync(User.Id(), cancellationToken);
        Scopes = request.Scope is { } scope ? scope.Split().ToList() : new List<string>();
        
        return Page();
    }
    
    public async Task<IActionResult> OnPostAsync(AuthorizeRequestModel request, [FromForm] bool cancel = false, CancellationToken cancellationToken = default)
    {
        var validation = await ValidateParametersAsync(request, cancellationToken);

        if (validation.Error is not null)
        {
            return validation.Error();
        }

        if (validation.Client is null || validation.Redirect is not { } redirect)
        {
            return PageError("something_happened");
        }

        if (cancel)
        {
            return RedirectToError(redirect, OAuth.Errors.AccessDenied, "User denied the request.", request.State);
        }

        return request.ResponseType switch
        {
            OAuth.ResponseTypes.Code => AuthorizationCode(validation.Client, redirect, request),
            OAuth.ResponseTypes.Token => await TokenAsync(validation.Client, redirect, request, cancellationToken),
            _ => RedirectToError(redirect, OAuth.Errors.UnsupportedResponseType, null, request.State)
        };
    }

    private IActionResult AuthorizationCode(IApplication client, IRedirect redirect, AuthorizeRequestModel request)
    {
        var protector = _dataProtectionProvider.CreateProtector("oauth");

        string code = protector.Protect(JsonSerializer.Serialize(new AuthorizationCodeModel
        {
            ClientId = client.Id,
            UserId = User.Id(),
            RedirectId = redirect.Id,

            RequireRedirect = client.Redirects.Count > 1,

            CodeChallenge = request.CodeChallenge,
            CodeChallengeMethod = request.CodeChallengeMethod,
            Scope = request.Scope
        }));

        var query = new QueryBuilder {{ OAuth.Parameters.Code, code }};

        if (request.State is not null)
        {
            query.Add(OAuth.Parameters.State, request.State);
        }

        return Redirect(redirect.Uri.OriginalString + query);
    }

    private async Task<IActionResult> TokenAsync(IApplication client, IRedirect redirect, AuthorizeRequestModel request, CancellationToken cancellationToken)
    {
        var connection = await _connections.GetOrCreateAsync(User.Id(), client.Id, false, cancellationToken);

        var session = await _sessions.CreateAsync(new CreateSessionBuilder()
            .WithConnection(connection.Id)
            .WithRedirect(redirect.Id)
            .WithScope(request.Scope)
            .WithCanRefresh(false), cancellationToken);
        
        var token = await _tokens.GenerateTokenAsync(session);

        int expiresIn = (int) (DateTime.UtcNow - token.Expiry).TotalSeconds;

        var query = new QueryBuilder
        {
            { OAuth.Parameters.AccessToken, token.AccessToken },
            { OAuth.Parameters.TokenType, token.TokenType },
            { OAuth.Parameters.ExpiresIn, expiresIn.ToString() }
        };

        if (request.State is not null)
        {
            query.Add(OAuth.Parameters.State, request.State);
        }

        string url = redirect.Uri.OriginalString + query;

        return Redirect(url.Replace('?', '#'));
    }
    
    private IActionResult RedirectToError(IRedirect redirect, string error, string? message, string? state)
    {
        var query = new QueryBuilder {{ OAuth.Parameters.Error, error }};
        
        if (message is not null)
        {
            query.Add(OAuth.Parameters.ErrorDescription, message);
        }

        if (state is not null)
        {
            query.Add(OAuth.Parameters.State, state);
        }
        
        return Redirect(redirect.Uri.OriginalString + query);
    }

    private IActionResult PageError(string error, string? message = null)
    {
        Error = new AuthorizeErrorModel(error, message);

        return Page();
    }
}