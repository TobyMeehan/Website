using FluentValidation;

namespace TobyMeehan.Com.Api.Features.Users.Password;

public class Validator : AbstractValidator<Request>
{
    public Validator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty();

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(100);
    }
}