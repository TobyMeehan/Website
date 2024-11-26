using FastEndpoints;
using FluentValidation;

namespace TobyMeehan.Com.Features.Downloads.Post;

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
            .IsInEnum().WithMessage("Visibility should be of type 'public' | 'private' | 'unlisted'.");
    }
}