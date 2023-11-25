using FluentValidation;

namespace TobyMeehan.Com.Api.Features.Applications.SetIcon;

public class Validator : AbstractValidator<Request>
{
    public Validator()
    {
        When(x => x.Icon is not null, () =>
        {
            RuleFor(x => x.Icon!.Length)
                .LessThanOrEqualTo(50 * 1024 * 1024);
        });
    }
}