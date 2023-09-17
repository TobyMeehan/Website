using System.Security.Claims;
using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using TobyMeehan.Com.Accounts.Authentication;
using TobyMeehan.Com.Accounts.Configuration;
using TobyMeehan.Com.Builders;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Accounts.Endpoints.Token;

public class TokenEndpoint : Endpoint<TokenRequest, Results<Ok<TokenResponse>, BadRequest<TokenErrorResponse>>>
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
        AuthSchemes(ClientBasicAuthenticationHandler.AuthenticationScheme);
        AllowAnonymous();
        DontThrowIfValidationFails();
    }

    public override async Task<Results<Ok<TokenResponse>, BadRequest<TokenErrorResponse>>> ExecuteAsync(TokenRequest req, CancellationToken ct)
    {
        if (ValidationFailed)
        {
            return TypedResults.BadRequest(new TokenErrorResponse
            {
                Error = ValidationFailures.First().ErrorCode,
                ErrorDescription = ValidationFailures.First().ErrorMessage
            });
        }
        
        var application = (User.Claims.ToList(), req) switch
        {
            ([{ Type: ClaimTypes.NameIdentifier, Value: { } id }], _)
                => await _applications.GetByIdAsync(new Id<IApplication>(id), ct),
            (_, { ClientId: { } clientId, ClientSecret: { } secret })
                => await _applications.GetByCredentialsAsync(new Id<IApplication>(clientId), secret, ct),
            (_, {ClientId: { } clientId})
                => await _applications.GetByCredentialsAsync(new Id<IApplication>(clientId), null, ct),
            _ => null
        };

        if (application is null)
        {
            return TypedResults.BadRequest(new TokenErrorResponse
            {
                Error = OAuth.Errors.InvalidClient,
                ErrorDescription = "Client credentials invalid or not provided."
            });
        }
        
        return req switch
        {
            { GrantType: OAuth.GrantTypes.AuthorizationCode } => await AuthorizationCodeAsync(req, application, ct),
            _ => throw new ArgumentOutOfRangeException(nameof(req), req, null)
        };
    }

    private async Task<Results<Ok<TokenResponse>, BadRequest<TokenErrorResponse>>> AuthorizationCodeAsync(
        TokenRequest req, IApplication application, CancellationToken ct)
    {
        var session = await _sessions.GetByCodeAsync(req.Code!, ct);

        if (session is null)
        {
            return TypedResults.BadRequest(new TokenErrorResponse
            {
                Error = OAuth.Errors.InvalidGrant,
                ErrorDescription = "Authorization code is invalid."
            });
        }

        if (application.Id != session.Application.Id)
        {
            return TypedResults.BadRequest(new TokenErrorResponse
            {
                Error = OAuth.Errors.InvalidGrant,
                ErrorDescription = "Authorization code is invalid."
            });
        }

        if (session.Redirect is not null && req.RedirectUri is null)
        {
            return TypedResults.BadRequest(new TokenErrorResponse
            {
                Error = OAuth.Errors.InvalidGrant,
                ErrorDescription = "Parameter redirect_uri is required for this request."
            });
        }

        if (session.Redirect?.Uri.OriginalString != req.RedirectUri)
        {
            return TypedResults.BadRequest(new TokenErrorResponse
            {
                Error = OAuth.Errors.InvalidGrant,
                ErrorDescription = "Redirect URI is invalid."
            });
        }

        return Token(await _sessions.StartAsync(session.Id, new StartSessionBuilder().WithCanRefresh(true), ct));
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