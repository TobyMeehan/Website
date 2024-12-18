using FastEndpoints;
using FluentValidation;

namespace TobyMeehan.Com.Features.Files.Patch;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Filename)
            .NotEmpty()
            .MaximumLength(128);
    }
}