using FastEndpoints;
using FluentValidation;

namespace TobyMeehan.Com.Features.Downloads.Put;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(40);

        RuleFor(x => x.Summary)
            .NotEmpty()
            .MaximumLength(400);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(4000);

        RuleFor(x => x.Visibility)
            .IsInEnum();

        RuleFor(x => x.Version)
            .Must(x => Version.TryParse(x, out _)).WithMessage("Not a valid version.")
            .Unless(x => string.IsNullOrEmpty(x.Version));
    }
}