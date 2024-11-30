using FastEndpoints;
using FluentValidation;

namespace TobyMeehan.Com.Features.Comments.Put;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .MaximumLength(400);
    }
}