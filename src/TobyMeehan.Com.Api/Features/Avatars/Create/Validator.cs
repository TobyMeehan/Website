using FluentValidation;

namespace TobyMeehan.Com.Api.Features.Avatars.Create;

public class Validator : AbstractValidator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Avatar.Length)
            .LessThanOrEqualTo(50 * 1024 * 1024);
    }
}