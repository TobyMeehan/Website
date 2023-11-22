using FluentValidation;

namespace TobyMeehan.Com.Api.Features.Application.Create;

public class Validator : AbstractValidator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Application name is required.")
            .MaximumLength(400)
            .WithMessage("Application length must be less than or equal to 400.");
    }
}