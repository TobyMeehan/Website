using FastEndpoints;
using FluentValidation;

namespace TobyMeehan.Com.Accounts.Endpoints.Token;

public class TokenRequest
{
    [BindFrom(OAuth.Parameters.GrantType)]
    public string GrantType { get; set; }
    
    [BindFrom(OAuth.Parameters.Code)]
    public string? Code { get; set; }
    
    [BindFrom(OAuth.Parameters.RedirectUri)]
    public string? RedirectUri { get; set; }
    
    [BindFrom(OAuth.Parameters.ClientId)]
    public string? ClientId { get; set; }
    
    [BindFrom(OAuth.Parameters.ClientSecret)]
    public string? ClientSecret { get; set; }

    [BindFrom(OAuth.Parameters.CodeVerifier)]
    public string? CodeVerifier { get; set; }
    
    [BindFrom(OAuth.Parameters.Scope)] 
    public string? Scope { get; set; }
}

public class TokenRequestValidator : Validator<TokenRequest>
{
    public TokenRequestValidator()
    {
        RuleFor(x => x.GrantType)
            .NotEmpty()
            .WithErrorCode(OAuth.Errors.InvalidRequest)
            .WithMessage("Parameter grant_type is required.")
            .Must(x => new[] { OAuth.GrantTypes.AuthorizationCode }.Contains(x))
            .WithErrorCode(OAuth.Errors.UnsupportedGrantType);

        RuleFor(x => x.Code)
            .NotEmpty()
            .When(x => x.GrantType == OAuth.GrantTypes.AuthorizationCode)
            .WithErrorCode(OAuth.Errors.InvalidRequest)
            .WithMessage("Parameter code is required.");
    }
}

public class TokenResponse
{
    public string AccessToken { get; set; }
    public string TokenType { get; set; }
    public int ExpiresIn { get; set; }
    public string? RefreshToken { get; set; }
    public string? Scope { get; set; }
}

public class TokenErrorResponse
{
    public string Error { get; set; }
    public string? ErrorDescription { get; set; }
    public string? ErrorUri { get; set; }
}