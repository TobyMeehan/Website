using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using TobyMeehan.Com.Accounts.Authentication;
using TobyMeehan.Com.Accounts.Configuration;
using TobyMeehan.Com.Accounts.Jwt;
using TobyMeehan.Com.Accounts.Models;
using TobyMeehan.Com.Builders;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Accounts.Endpoints.Token;

public class TokenEndpoint : Endpoint<TokenRequest, Results<Ok<TokenResponse>, BadRequest<TokenErrorResponse>, UnauthorizedHttpResult>>
{
    private readonly ISessionService _sessions;
    private readonly IApplicationService _applications;
    private readonly IConnectionService _connections;
    private readonly ITokenService _tokens;
    private readonly IDataProtectionProvider _dataProtectionProvider;

    public TokenEndpoint(ISessionService sessions, IApplicationService applications, IConnectionService connections, ITokenService tokens, IDataProtectionProvider dataProtectionProvider)
    {
        _sessions = sessions;
        _applications = applications;
        _connections = connections;
        _tokens = tokens;
        _dataProtectionProvider = dataProtectionProvider;
    }
    
    public override void Configure()
    {
        Post("/oauth/token");
        AllowFormData(urlEncoded: true);
        AllowAnonymous();
        DontThrowIfValidationFails();
    }

    private (bool Valid, string Scheme, string? ClientId, string? ClientSecret) GetAuthenticationFromHeader()
    {
        (bool Valid, string Scheme, string? ClientId, string? ClientSecret) result = (true, string.Empty, null, null);

        if (HttpContext.Request.Headers.Authorization is var header && header == StringValues.Empty)
        {
            return result;
        }
        
        if (!AuthenticationHeaderValue.TryParse(header, out var headerValue))
        {
            return result with { Valid = false };
        }

        result.Scheme = headerValue.Scheme;
        
        if (headerValue is not { Scheme: "Basic", Parameter: { } parameter })
        {
            return result with { Valid = false };
        }

        string credentials = Encoding.UTF8.GetString(Convert.FromBase64String(parameter));

        if (credentials.Split(":") is not [{ } clientId, { } clientSecret])
        {
            return result with { Valid = false };
        }

        return result with { ClientId = clientId, ClientSecret = clientSecret };
    }
    
    public override async Task<Results<Ok<TokenResponse>, BadRequest<TokenErrorResponse>, UnauthorizedHttpResult>> ExecuteAsync(TokenRequest req, CancellationToken ct)
    {
        if (ValidationFailed)
        {
            return TypedResults.BadRequest(new TokenErrorResponse
            {
                Error = ValidationFailures.First().ErrorCode,
                ErrorDescription = ValidationFailures.First().ErrorMessage
            });
        }

        var authentication = GetAuthenticationFromHeader();

        if (authentication is { Valid: false, Scheme: var scheme })
        {
            HttpContext.Response.Headers.WWWAuthenticate = scheme;
            return TypedResults.Unauthorized();
        }
        
        (string? clientId, string? clientSecret) = (authentication.ClientId, authentication.ClientSecret);

        clientId ??= req.ClientId;
        clientSecret ??= req.ClientSecret;

        if (string.IsNullOrEmpty(clientId))
        {
            return TypedResults.BadRequest(new TokenErrorResponse
            {
                Error = OAuth.Errors.InvalidClient,
                ErrorDescription = "Client credentials not provided."
            });
        }

        return req switch
        {
            { GrantType: OAuth.GrantTypes.AuthorizationCode, Code: { } code }
                => await AuthorizationCodeAsync(clientId, code, req.RedirectUri, clientSecret, req.CodeVerifier, ct),
            { GrantType: OAuth.GrantTypes.ClientCredentials }
                => await ClientCredentialsAsync(clientId, clientSecret, req.Scope, ct),
            _ => TypedResults.BadRequest(new TokenErrorResponse { Error = OAuth.Errors.InvalidGrant })
        };
    }

    private async Task<Results<Ok<TokenResponse>, BadRequest<TokenErrorResponse>, UnauthorizedHttpResult>> AuthorizationCodeAsync(
        string clientId, string code, string? redirectUri, string? secret, string? codeVerifier, CancellationToken ct)
    {
        var protector = _dataProtectionProvider.CreateProtector("oauth");

        var authorization = JsonSerializer.Deserialize<AuthorizationCodeModel>(protector.Unprotect(code));
        
        if (authorization is null)
        {
            return TypedResults.BadRequest(new TokenErrorResponse
            {
                Error = OAuth.Errors.InvalidGrant,
                ErrorDescription = "Authorization code is invalid."
            });
        }

        if (clientId != authorization.ClientId.Value)
        {
            return TypedResults.BadRequest(new TokenErrorResponse
            {
                Error = OAuth.Errors.InvalidClient,
                ErrorDescription = "Client credentials invalid."
            });
        }
        
        if (string.IsNullOrEmpty(secret) && string.IsNullOrEmpty(codeVerifier))
        {
            return TypedResults.BadRequest(new TokenErrorResponse
            {
                Error = OAuth.Errors.InvalidClient,
                ErrorDescription = "Parameter client_secret is required."
            });
        }

        var application = string.IsNullOrEmpty(secret)
            ? await _applications.GetByIdAsync(authorization.ClientId, ct)
            : await _applications.GetByCredentialsAsync(authorization.ClientId, secret, ct);
        
        if (application is null)
        {
            return TypedResults.BadRequest(new TokenErrorResponse
            {
                Error = OAuth.Errors.InvalidClient,
                ErrorDescription = "Client credentials invalid."
            });
        }
        
        if (authorization.CodeChallenge is { Length: > 0 } codeChallenge && !string.IsNullOrEmpty(codeVerifier)
                                                       && !ValidateCodeVerifier(codeChallenge, codeVerifier))
        {
            return TypedResults.BadRequest(new TokenErrorResponse
            {
                Error = OAuth.Errors.InvalidGrant,
                ErrorDescription = "Invalid code_verifier"
            });
        }

        var redirect = application.Redirects[authorization.RedirectId];

        if (authorization.RequireRedirect && string.IsNullOrEmpty(redirectUri))
        {
            return TypedResults.BadRequest(new TokenErrorResponse
            {
                Error = OAuth.Errors.InvalidGrant,
                ErrorDescription = "Parameter redirect_uri is required for this request."
            });
        }

        if (redirect?.Uri.OriginalString != redirectUri)
        {
            return TypedResults.BadRequest(new TokenErrorResponse
            {
                Error = OAuth.Errors.InvalidGrant,
                ErrorDescription = "Redirect URI is invalid."
            });
        }

        var connection = await _connections.GetOrCreateAsync(authorization.UserId, application.Id, false, ct);

        var session = await _sessions.CreateAsync(new CreateSessionBuilder()
            .WithConnection(connection.Id)
            .WithRedirect(authorization.RedirectId)
            .WithScope(authorization.Scope)
            .WithCanRefresh(true), ct);

        var token = await _tokens.GenerateTokenAsync(session);
        
        return TypedResults.Ok(new TokenResponse
        {
            AccessToken = token.AccessToken,
            TokenType = token.TokenType,
            ExpiresIn = (int) (token.Expiry - DateTime.UtcNow).TotalSeconds,
            RefreshToken = token.RefreshToken,
            Scope = string.Join(' ', session.Scope)
        });
    }

    private bool ValidateCodeVerifier(string codeChallenge, string codeVerifier)
    {
        return OAuth.Base64UrlEncode(SHA256.HashData(Encoding.UTF8.GetBytes(codeVerifier))) == codeChallenge;
    }

    private async Task<Results<Ok<TokenResponse>, BadRequest<TokenErrorResponse>, UnauthorizedHttpResult>> ClientCredentialsAsync(
            string clientId, string? secret, string? scope, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(secret) 
            || await _applications.GetByCredentialsAsync(new Id<IApplication>(clientId), secret, ct) is not { } application)
        {
            return TypedResults.BadRequest(new TokenErrorResponse
            {
                Error = OAuth.Errors.InvalidClient,
                ErrorDescription = "Client credentials invalid."
            });
        }

        var connection = await _connections.GetOrCreateAsync(application.AuthorId, application.Id, false, ct);

        var session = await _sessions.CreateAsync(new CreateSessionBuilder()
            .WithConnection(connection.Id)
            .WithScope(scope)
            .WithCanRefresh(false), ct);

        var token = await _tokens.GenerateTokenAsync(session);

        return TypedResults.Ok(new TokenResponse
        {
            AccessToken = token.AccessToken,
            TokenType = token.TokenType,
            ExpiresIn = (int) (token.Expiry - DateTime.UtcNow).TotalSeconds,
            Scope = scope
        });
    }
}