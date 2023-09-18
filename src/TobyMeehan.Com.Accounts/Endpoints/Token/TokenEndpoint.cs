using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using TobyMeehan.Com.Accounts.Authentication;
using TobyMeehan.Com.Accounts.Configuration;
using TobyMeehan.Com.Builders;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Accounts.Endpoints.Token;

public class TokenEndpoint : Endpoint<TokenRequest, Results<Ok<TokenResponse>, BadRequest<TokenErrorResponse>, UnauthorizedHttpResult>>
{
    private readonly ISessionService _sessions;
    private readonly IApplicationService _applications;
    private readonly AuthenticationOptions _options;

    public TokenEndpoint(ISessionService sessions, IApplicationService applications, IOptions<AuthenticationOptions> options)
    {
        _sessions = sessions;
        _applications = applications;
        _options = options.Value;
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
            _ => TypedResults.BadRequest(new TokenErrorResponse { Error = OAuth.Errors.InvalidGrant })
        };
    }

    private async Task<Results<Ok<TokenResponse>, BadRequest<TokenErrorResponse>, UnauthorizedHttpResult>> AuthorizationCodeAsync(
        string clientId, string code, string? redirect, string? secret, string? codeVerifier, CancellationToken ct)
    {
        var session = await _sessions.GetByCodeAsync(code, ct);
        
        if (session is null)
        {
            return TypedResults.BadRequest(new TokenErrorResponse
            {
                Error = OAuth.Errors.InvalidGrant,
                ErrorDescription = "Authorization code is invalid."
            });
        }

        if (clientId != session.Application.Id.Value)
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

        if (!string.IsNullOrEmpty(secret) &&
            await _applications.GetByCredentialsAsync(new Id<IApplication>(clientId), secret, ct) is null)
        {
            return TypedResults.BadRequest(new TokenErrorResponse
            {
                Error = OAuth.Errors.InvalidClient,
                ErrorDescription = "Client credentials invalid."
            });
        }
        
        if (session.CodeChallenge is { Length: > 0 } codeChallenge && !string.IsNullOrEmpty(codeVerifier)
                                                       && !ValidateCodeVerifier(codeChallenge, codeVerifier))
        {
            return TypedResults.BadRequest(new TokenErrorResponse
            {
                Error = OAuth.Errors.InvalidGrant,
                ErrorDescription = "Invalid code_verifier"
            });
        }

        if (session.Redirect is not null && string.IsNullOrEmpty(redirect))
        {
            return TypedResults.BadRequest(new TokenErrorResponse
            {
                Error = OAuth.Errors.InvalidGrant,
                ErrorDescription = "Parameter redirect_uri is required for this request."
            });
        }

        if (session.Redirect?.Uri.OriginalString != redirect)
        {
            return TypedResults.BadRequest(new TokenErrorResponse
            {
                Error = OAuth.Errors.InvalidGrant,
                ErrorDescription = "Redirect URI is invalid."
            });
        }

        return Token(await _sessions.StartAsync(session.Id, new StartSessionBuilder().WithCanRefresh(true), ct));
    }

    private bool ValidateCodeVerifier(string codeChallenge, string codeVerifier)
    {
        return OAuth.Base64UrlEncode(SHA256.HashData(Encoding.UTF8.GetBytes(codeVerifier))) == codeChallenge;
    }

    private Ok<TokenResponse> Token(ISession session)
    {
        string token = JWTBearer.CreateToken(
            signingKey: _options.Jwt.TokenSigningKey,
            expireAt: session.Expiry,
            audience: session.Application.Id.Value,
            privileges: u =>
            {
                u["sub"] = session.User.Id.Value;
                u["SessionId"] = session.Id.Value;
            });
        
        return TypedResults.Ok(new TokenResponse
        {
            AccessToken = token,
            TokenType = "Bearer",
            ExpiresIn = (int) (session.Expiry - DateTime.UtcNow).TotalSeconds,
            RefreshToken = session.RefreshToken
        });
    }
}