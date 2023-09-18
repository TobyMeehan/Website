using System.Security.Cryptography;
using System.Text.Json;
using System.Web;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TobyMeehan.Com.Accounts.Extensions;
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

    public Authorize(IApplicationService applications, IUserService users, IConnectionService connections, ISessionService sessions, IDataProtectionProvider dataProtectionProvider)
    {
        _applications = applications;
        _users = users;
        _connections = connections;
        _sessions = sessions;
        _dataProtectionProvider = dataProtectionProvider;
    }

    public AuthorizeErrorModel? Error { get; set; }
    
    public IApplication? Client { get; set; }
    public IUser? Owner { get; set; }
    public List<string> Scopes { get; set; } = new();

    public string? ReturnUrl { get; set; }
    [BindProperty] public string RequestId { get; set; } = "";
    
    public async Task<IActionResult> OnGetAsync(
        [FromQuery(Name = OAuth.Parameters.ResponseType)] string responseType,
        [FromQuery(Name = OAuth.Parameters.ClientId)] string clientId,
        [FromQuery(Name = OAuth.Parameters.CodeChallenge)] string? codeChallenge,
        [FromQuery(Name = OAuth.Parameters.CodeChallengeMethod)] string? codeChallengeMethod = null,
        [FromQuery(Name = OAuth.Parameters.RedirectUri)] string? redirectUri = null,
        [FromQuery(Name = OAuth.Parameters.Scope)] string? scope = null,
        [FromQuery(Name = OAuth.Parameters.State)] string? state = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(clientId))
        {
            return PageError("client_id");
        }

        var client = await _applications.GetByIdAsync(new Id<IApplication>(clientId), cancellationToken);

        if (client is null)
        {
            return PageError("client_id");
        }

        var redirect = redirectUri switch
        {
            null when client.Redirects.Count == 1 => client.Redirects.Single(),

            not null => client.Redirects.FirstOrDefault(x => x.Uri.OriginalString == redirectUri),

            _ => null
        };

        if (redirect is null)
        {
            return PageError("redirect_uri");
        }

        if (!new[] { OAuth.ResponseTypes.Code }.Contains(responseType))
        {
            return RedirectToError(redirect, OAuth.Errors.UnsupportedResponseType, $"Response type {responseType} is not supported.", state);
        }
        
        if (!string.IsNullOrEmpty(scope) && scope.Split().Except(Scope.All).Any())
        {
            return RedirectToError(redirect, OAuth.Errors.InvalidScope, null, state);
        }

        if (codeChallenge is not null && codeChallengeMethod is not OAuth.Transformations.S256)
        {
            return RedirectToError(redirect, OAuth.Errors.InvalidRequest, "Transform algorithm not supported", state);
        }

        ReturnUrl = Url.Page(nameof(Authorize), new
        {
            response_type = responseType,
            client_id = clientId,
            code_challenge = codeChallenge,
            code_challenge_method = codeChallengeMethod,
            redirect_uri = redirectUri,
            scope,
            state
        });
        
        if (User.Identity?.IsAuthenticated is not true)
        {
            return RedirectToPage(nameof(Login), new { ReturnUrl });
        }
        
        Client = client;
        Owner = await _users.GetByIdAsync(User.Id(), cancellationToken);
        Scopes = scope is not null ? scope.Split().ToList() : new List<string>();

        var protector = _dataProtectionProvider.CreateProtector("oauth");

        var request = new AuthorizeRequestModel
        {
            ResponseType = responseType,
            ClientId = client.Id,
            CodeChallenge = codeChallenge,
            CodeChallengeMethod = codeChallengeMethod,
            RedirectId = redirect.Id,
            Scope = scope,
            State = state
        };

        RequestId = protector.Protect(JsonSerializer.Serialize(request));
        
        return Page();
    }
    
    public async Task<IActionResult> OnPostCancel()
    {
        var protector = _dataProtectionProvider.CreateProtector("oauth");

        var request = JsonSerializer.Deserialize<AuthorizeRequestModel>(protector.Unprotect(RequestId));

        if (request is null)
        {
            return PageError("something_happened");
        }

        var client = await _applications.GetByIdAsync(request.ClientId);

        if (client is null)
        {
            return PageError("something_happened");
        }
        
        var redirect = client.Redirects[request.RedirectId];

        if (redirect is null)
        {
            return PageError("something_happened");
        }
        
        return RedirectToError(redirect, OAuth.Errors.AccessDenied, "User denied the request.", request.State);
    }
    
    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken = default)
    {
        var protector = _dataProtectionProvider.CreateProtector("oauth");

        AuthorizeRequestModel? request;

        try
        {
            request = JsonSerializer.Deserialize<AuthorizeRequestModel>(protector.Unprotect(RequestId));
        }
        catch (CryptographicException)
        {
            request = null;
        }

        if (request is null)
        {
            return PageError("something_happened");
        }
        
        var connection = await _connections.GetOrCreateAsync(User.Id(), request.ClientId, false, cancellationToken);

        var redirect = connection.Application.Redirects[request.RedirectId];

        if (redirect is null)
        {
            return PageError("something_happened");
        }
        
        var session = await _sessions.CreateAsync(new CreateSessionBuilder()
            .WithConnection(connection.Id)
            .WithRedirect(redirect.Id)
            .WithScope(request.Scope)
            .WithCodeChallenge(request.CodeChallenge), cancellationToken);

        var query = new QueryBuilder {{ OAuth.Parameters.Code, session.AuthorizationCode }};

        if (request.State is not null)
        {
            query.Add(OAuth.Parameters.State, request.State);
        }

        return Redirect(redirect.Uri.OriginalString + query);
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