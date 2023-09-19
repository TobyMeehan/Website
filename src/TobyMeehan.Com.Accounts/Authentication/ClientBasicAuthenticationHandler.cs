using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Accounts.Authentication;

public class ClientBasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IApplicationService _applications;

    public const string AuthenticationScheme = "ClientBasic";

    public ClientBasicAuthenticationHandler(IApplicationService applications, IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
    {
        _applications = applications;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var endpoint = Context.GetEndpoint();

        if (endpoint?.Metadata.GetMetadata<IAllowAnonymous>() is not null)
        {
            return AuthenticateResult.NoResult();
        }

        if (!Request.Headers.ContainsKey("Authorization"))
        {
            return AuthenticateResult.Fail("Missing Authorization header.");
        }

        string username, password;

        try
        {
            var header = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);

            string[] credentials = Encoding.UTF8.GetString(
                    Convert.FromBase64String(header.Parameter! /* catch will handle null value here */))
                .Split(":");

            (username, password) = (credentials[0], credentials[1]);
        }
        catch
        {
            return AuthenticateResult.Fail("Invalid Authorization header.");
        }
        
        var application = await _applications.FindByCredentialsAsync(username, password);

        if (application is null)
        {
            return AuthenticateResult.Fail("Invalid application credentials.");
        }
        
        var claims = new[] 
        {
            new Claim(ClaimTypes.NameIdentifier, application.Id.Value)
        };

        var principle = new ClaimsPrincipal(new ClaimsIdentity(claims, AuthenticationScheme));
        
        return AuthenticateResult.Success(new AuthenticationTicket(principle, AuthenticationScheme));
    }
}