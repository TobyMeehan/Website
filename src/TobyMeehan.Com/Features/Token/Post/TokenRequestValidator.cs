using FastEndpoints;
using FluentValidation;

namespace TobyMeehan.Com.Features.Token.Post;

public class TokenRequestValidator : Validator<TokenRequest>
{
    public TokenRequestValidator()
    {
        RuleFor(x => x.AccessToken)
            .NotEmpty();
    }
}