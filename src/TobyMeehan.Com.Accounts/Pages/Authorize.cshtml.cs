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
        [FromQuery(Name = "response_type")] string responseType,
        [FromQuery(Name = "client_id")] string clientId,
        [FromQuery(Name = "code_challenge")] string? codeChallenge,
        [FromQuery(Name = "code_challenge_method")] string? codeChallengeMethod = null,
        [FromQuery(Name = "redirect_uri")] string? redirectUri = null,
        [FromQuery(Name = "scope")] string? scope = null,
        [FromQuery(Name = "state")] string? state = null,
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

        if (!new[] { "code" }.Contains(responseType))
        {
            return RedirectToError(redirect, "unsupported_response_type", $"Response type {responseType} is not supported.", state);
        }

        if (!string.IsNullOrEmpty(scope) && scope.Split().Except(Scope.All).Any())
        {
            return RedirectToError(redirect, "invalid_scope", null, state);
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
        
        if (!(User.Identity?.IsAuthenticated ?? false))
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
        
        return RedirectToError(redirect, "access_denied", "User denied the request.", request.State);
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

        var query = new QueryBuilder {{"code", session.AuthorizationCode}};

        if (request.State is not null)
        {
            query.Add("state", request.State);
        }

        return Redirect(connection.Application.Redirects[request.RedirectId]?.Uri.OriginalString + query);
    }
    
    private IActionResult RedirectToError(IRedirect redirect, string error, string? message, string? state)
    {
        var query = new QueryBuilder {{"error", error}};
        
        if (message is not null)
        {
            query.Add("error_description", message);
        }

        if (state is not null)
        {
            query.Add("state", state);
        }
        
        return Redirect(redirect.Uri.OriginalString + query);
    }

    private IActionResult PageError(string error, string? message = null)
    {
        Error = new AuthorizeErrorModel(error, message);

        return Page();
    }
}