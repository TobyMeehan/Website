using FluentValidation;

namespace TobyMeehan.Com.Api.Features.Downloads.Create;

public class Validator : AbstractValidator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .Length(1, 40);

        RuleFor(x => x.Summary)
            .NotEmpty()
            .Length(1, 100);

        When(x => x.Description.HasValue, () =>
        {
            RuleFor(x => x.Description.Value)
                .Length(1, 400);
        });

        When(x => x.Visibility.HasValue, () =>
        {
            RuleFor(x => x.Visibility.Value)
                .NotEmpty()
                .Must(x =>
                    new[] { VisibilityNames.Public, VisibilityNames.Unlisted, VisibilityNames.Private }.Contains(x));
        });
    }
}