using System.Security.Claims;
using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.Extensions.Options;
using TobyMeehan.Com.Domain.Thavyra.Services;
using TobyMeehan.Com.Security.Configuration;

namespace TobyMeehan.Com.Features.Token.Post;

public class TokenEndpoint : Endpoint<TokenRequest, TokenResponse>
{
    private readonly IUserService _userService;
    private readonly JwtOptions _jwtOptions;

    public TokenEndpoint(IUserService userService, IOptions<JwtOptions> jwtOptions)
    {
        _userService = userService;
        _jwtOptions = jwtOptions.Value;
    }
    
    public override void Configure()
    {
        Post("/token");
        AllowAnonymous();
    }

    public override async Task HandleAsync(TokenRequest req, CancellationToken ct)
    {
        var user = await _userService.GetByAccessTokenAsync(req.AccessToken, ct);

        if (user is null)
        {
            ThrowError(x => x.AccessToken, "Could not retrieve user.");
        }
        
        var expiry = DateTime.UtcNow.AddHours(1);

        var token = JwtBearer.CreateToken(options =>
        {
            options.SigningKey = _jwtOptions.Key!;
            
            options.ExpireAt = expiry;

            options.User[ClaimTypes.NameIdentifier] = user.Id.ToString();
        });

        Response = new TokenResponse
        {
            Token = token,
            ExpiresIn = Convert.ToInt32((DateTime.UtcNow - expiry).TotalSeconds)
        };
    }
}